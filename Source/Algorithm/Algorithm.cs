using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRL.Commons.Utilities;
using SRL.Commons.Model;
using System.Windows;
using SRL.Commons;
using SRL.Commons.Model.Base;
using System.Threading;
using ASD.Graph;

namespace SRL.Algorithm
{
    public class Algorithm : IAlgorithm
    {
        private List<Option> OptionTemplates;
        private List<Option> CurrentOptions;

        struct IndexPoint // structure used to unify different index with each point
        {
            public Point point;
            public int index;
            public int obstacle;
        }
        public List<Order> GetPath(Map InputMap, Vehicle InputVehicle, Point start, Point end, double vehicleSize, double vehicleRotation, CancellationToken token)
        {
            // default values for user options
            int turnEdgeWeight = 10;
            double maxDiff = 0.01;
            int angleDensity = 360;
            bool backwards = true;
            bool allDirections = true;


            // wczytanie opcji
            foreach (Option option in CurrentOptions)
            {
                switch (option.Names[Language.English])
                {
                    case "Angle density":
                        angleDensity = (int)option.Value;
                        break;
                    case "Point size":
                        maxDiff = (double)option.Value;
                        break;
                    case "Graph edge weight for turns":
                        turnEdgeWeight = (int)option.Value;
                        break;
                    case "Allow reverse":
                        backwards = (bool)option.Value;
                        break;
                    case "Allow vehicle to move in any direction":
                        allDirections = (bool)option.Value;
                        break;
                }
            }


            double singleAngle = 2 * Math.PI / angleDensity;
            // changing size of the vehicle to current one
            List<Point> lst = new List<Point>();
            for (int i = 0; i < InputVehicle.Shape.Vertices.Count; i++)
            {
                lst.Add(new Point(InputVehicle.Shape.Vertices[i].X * vehicleSize, InputVehicle.Shape.Vertices[i].Y * vehicleSize));
            }
            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(lst);

            // copy of the map with additional obstacles as map edges (commented at the moment) (size of those additional obstacles can be changed, it works fine for these ones), right now map is without bounds as we have trouble with vehicles moving by normal obstacles
            Map map = new Map();
            List<List<IndexPoint>>[] iPointObstacles = new List<List<IndexPoint>>[angleDensity];
            /*map.Obstacles.Add(new Polygon(new Point[] { new Point(-1, -2), new Point(-1, 2), new Point(-2, 2), new Point(-2, -2) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(-2, -1), new Point(2, -1), new Point(2, -2), new Point(-2, -2) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(1, 2), new Point(1, -2), new Point(2, -2), new Point(2, 2) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(2, 1), new Point(-2, 1), new Point(-2, 2), new Point(2, 2) }));
            */
            List<IndexPoint>[] IndexPointAngleList = new List<IndexPoint>[angleDensity];
            for (int i = 0; i < InputMap.Obstacles.Count; i++)
                map.Obstacles.Add(InputMap.Obstacles[i]);

            // Minkowski's sum calculation for each angle
            List<Polygon>[] currentMap = MinkowskiSum(map, vehicle, angleDensity);

            // creating triangle to verify, if points are linear later on
            List<Point> triangleTemplate = new List<Point>();
            triangleTemplate.Add(new Point(0, 0));
            triangleTemplate.Add(new Point(4, 4 * Math.Tan(singleAngle / 2)));
            triangleTemplate.Add(new Point(4, -4 * Math.Tan(singleAngle / 2)));
            Polygon triangle = new Polygon(triangleTemplate);

            // Getting angle for starting set up of the vehicle
            int startingIndex = (int)(((vehicleRotation + 2 * Math.PI) % (2 * Math.PI)) / singleAngle);


            // setting index for each Minkowski's sum point of each angle
            int index = 0;
            for (int i = 0; i < angleDensity; i++)
            {
                if (i == startingIndex)
                    startingIndex = index; // setting the index of starting point
                IndexPointAngleList[i] = new List<IndexPoint>();
                int vertices = 2;
                for (int j = 0; j < currentMap[i].Count; j++)
                    vertices += currentMap[i][j].Vertices.Count;
                IndexPoint ip = new IndexPoint();
                ip.index = index++;
                ip.point = start;
                ip.obstacle = -1;
                IndexPointAngleList[i].Add(ip);
                int k = 0;
                foreach (Polygon poly in currentMap[i])
                {
                    foreach (Point p in poly.Vertices)
                    {
                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException();
                        ip.point = p;
                        ip.index = index++;
                        ip.obstacle = k;
                        IndexPointAngleList[i].Add(ip);
                    }
                    k++;
                }
                ip.point = end;
                ip.index = index++;
                ip.obstacle = -1;
                IndexPointAngleList[i].Add(ip);
            }

            // creating a graph with vertices count equal to number of all indexed points enlarged by one (the accepting state for A* algorithm)
            IGraph graph = new AdjacencyListsGraph<HashTableAdjacencyList>(true, index + 1);

            // creating graph edges for each angle (all done in one graph from the very beginning)
            object locker = new object();
            Parallel.For(0, angleDensity, angle =>
              {
                  for (int i = 0; i < IndexPointAngleList[angle].Count; i++)
                  {
                      for (int j = 0; j < IndexPointAngleList[angle].Count; j++)
                      {
                          if (token.IsCancellationRequested)
                              return;
                          // Chcecking if starting and ending points for certain angle are not in that Minkowski's sum obstacles
                          if (IndexPointAngleList[angle][i].obstacle == -1)
                          {
                              bool cancel = false;
                              foreach (Polygon obstacle in currentMap[angle])
                              {
                                  if (GeometryHelper.IsEnclosed(IndexPointAngleList[angle][i].point, obstacle))
                                  {
                                      cancel = true;
                                      break;
                                  }
                              }
                              if (cancel)
                                  continue;
                          }
                          if (IndexPointAngleList[angle][j].obstacle == -1)
                          {
                              bool cancel = false;
                              foreach (Polygon obstacle in currentMap[angle])
                              {
                                  if (GeometryHelper.IsEnclosed(IndexPointAngleList[angle][j].point, obstacle))
                                  {
                                      cancel = true;
                                      break;
                                  }
                              }
                              if (cancel)
                                  continue;
                          }
                          if (i == j) continue; // We are not accepting edges in one point when not turning

                          if (CanTwoPointsConnect(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point, currentMap[angle], angle * singleAngle))
                          {
                              if (!allDirections)
                              {
                                  // if the Point that we are going to moce to is inside the triangle turned by the current angle, we can add an edge
                                  if (IsPointInTriangle(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point, angle * singleAngle, triangle))
                                  {
                                      graph.AddEdge(new Edge(IndexPointAngleList[angle][i].index, IndexPointAngleList[angle][j].index, GetEdgeWeight(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point)));
                                      // if user enabled reverse in options, we add an edge back
                                      if (backwards)
                                          graph.AddEdge(new Edge(IndexPointAngleList[angle][j].index, IndexPointAngleList[angle][i].index, GetEdgeWeight(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point)));
                                  }
                              }
                              // if user enabled all directions, we add all edges that passed all previous tests
                              else
                                  graph.AddEdge(new Edge(IndexPointAngleList[angle][i].index, IndexPointAngleList[angle][j].index, GetEdgeWeight(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point)));
                          }
                      }
                  }
              });

            if (token.IsCancellationRequested)
                throw new OperationCanceledException();

            // Adding turning edges
            for (int angle = 0; angle < angleDensity; angle++)
            {
                for (int i = 0; i < IndexPointAngleList[angle].Count; i++)
                {
                    for (int j = 0; j < IndexPointAngleList[(angle + 1) % angleDensity].Count; j++)
                    {
                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException();
                        // Again, checking if starting and ending point are not in any Minkowski's sum polygons
                        if (IndexPointAngleList[angle][i].obstacle == -1)
                        {
                            bool cancel = false;
                            foreach (Polygon obstacle in currentMap[angle])
                            {
                                if (GeometryHelper.IsEnclosed(IndexPointAngleList[angle][i].point, obstacle))
                                {
                                    cancel = true;
                                    break;
                                }
                            }
                            if (cancel)
                                continue;
                        }
                        if (IndexPointAngleList[(angle + 1) % angleDensity][j].obstacle == -1)
                        {
                            bool cancel = false;
                            foreach (Polygon obstacle in currentMap[(angle + 1) % angleDensity])
                            {
                                if (GeometryHelper.IsEnclosed(IndexPointAngleList[(angle + 1) % angleDensity][j].point, obstacle))
                                {
                                    cancel = true;
                                    break;
                                }
                            }
                            if (cancel)
                                continue;
                        }
                        if (GeometryHelper.GetDistance(IndexPointAngleList[angle][i].point, IndexPointAngleList[(angle + 1) % angleDensity][j].point) <= maxDiff)
                        {
                            graph.AddEdge(IndexPointAngleList[angle][i].index, IndexPointAngleList[(angle + 1) % angleDensity][j].index, turnEdgeWeight);
                            graph.AddEdge(IndexPointAngleList[(angle + 1) % angleDensity][j].index, IndexPointAngleList[angle][i].index, turnEdgeWeight);
                        }
                    }
                }
            }

            // Adding edges to accepting state
            for (int i = 0; i < IndexPointAngleList.Length; i++)
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (IndexPointAngleList[i][IndexPointAngleList[i].Count - 1].obstacle == -1 && IndexPointAngleList[i][IndexPointAngleList[i].Count - 1].point == end)
                    graph.AddEdge(IndexPointAngleList[i][IndexPointAngleList[i].Count - 1].index, index, 0);
            }

            // Graph algorithm A*
            Edge[] path;
            AStarGraphExtender.AStar(graph, startingIndex, graph.VerticesCount - 1, out path);
            if (path == null)
                throw new NonexistentPathException();

            // Creating Orders from A* results
            // TODO: still some angle troubles
            List<Order> orders = new List<Order>();
            orders.Add(new Order() { Destination = start, Rotation = (vehicleRotation + 2 * Math.PI) % (2 * Math.PI) });
            for (int i = 0; i < path.Length - 1; i++)
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException();
                int angle = 0;
                while (path[i].To > IndexPointAngleList[angle][IndexPointAngleList[angle].Count - 1].index)
                    angle++;
                int ind = 0;
                while (IndexPointAngleList[angle][ind].index != path[i].To)
                    ind++;
                Order o = new Order();
                o.Destination = IndexPointAngleList[angle][ind].point;
                o.Rotation = angle * singleAngle;

                if ((o.Rotation + 2 * Math.PI) % (2 * Math.PI) == (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI))
                {
                    o.Rotation = (orders[orders.Count - 1].Rotation);
                    orders.Add(o);
                    continue;
                }

                if (o.Rotation == 0 && orders[orders.Count - 1].Rotation <= -Math.PI)
                {
                    o.Rotation = -2 * Math.PI;
                }
                else if (o.Rotation == 0 && orders[orders.Count - 1].Rotation > 0)
                {
                    // do nothing
                }
                else if (o.Rotation > 0 && orders[orders.Count - 1].Rotation > 0)
                {
                    if (o.Rotation < orders[orders.Count - 1].Rotation)
                        o.Rotation -= 2 * Math.PI;
                    // else do nothing
                }
                else if (o.Rotation > 0 && orders[orders.Count - 1].Rotation < 0)
                {
                    if (((o.Rotation + 2 * Math.PI) % (2 * Math.PI) < (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI)))
                        o.Rotation -= 2 * Math.PI;
                    else if (orders[orders.Count - 1].Rotation == -2 * Math.PI && o.Rotation > Math.PI)
                        o.Rotation -= 2 * Math.PI;
                    // else do nothing
                }
                else if (o.Rotation > 0 && orders[orders.Count - 1].Rotation == 0)
                {
                    if (o.Rotation > Math.PI)
                        o.Rotation -= 2 * Math.PI;
                }



                /*else if (((o.Rotation + 2 * Math.PI) % (2 * Math.PI) < (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI) 
                    || ((o.Rotation + (2 * Math.PI)) % (2 * Math.PI) > Math.Abs(orders[orders.Count - 1].Rotation + (2 * Math.PI)) % (2 * Math.PI) && ((orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI) == 0))))
                {
                        o.Rotation -= (2 * Math.PI);
                }*/

                orders.Add(o);
            }

            //return orders;

            // Checking for doubled points previous orders list
            List<Order> os = new List<Order>();
            os.Add(orders[0]);
            orders.RemoveAt(0);
            while (orders.Count > 0)
            {
                while (orders[0].Destination == os[os.Count - 1].Destination)
                {
                    //os[os.Count - 1].Rotation = orders[0].Rotation;
                    orders.RemoveAt(0);
                }
                os.Add(orders[0]);
                orders.RemoveAt(0);

            }
            os.RemoveAt(0);
            return os;
        }


        public Algorithm()
        {
            // Generating default options on class create
            GenerateOptions();
        }


        private void GenerateOptions()
        {
            CurrentOptions = new List<Option>();
            OptionTemplates = new List<Option>();

            // ANGLEDENSITY
            Option angleDensity = new Option();
            Dictionary<Language, string> angleDensityName = new Dictionary<Language, string>();
            Dictionary<Language, string> angleDensityToolTip = new Dictionary<Language, string>();
            angleDensity.Type = Option.ValueType.Integer;
            angleDensity.Value = 360;
            angleDensity.MinValue = 3;
            angleDensity.MaxValue = null;
            angleDensityName.Add(Language.English, "Angle density");
            angleDensityName.Add(Language.Polish, "Gęstość kątów");
            angleDensity.Names = angleDensityName;
            angleDensityToolTip.Add(Language.English, "Describes, how many units will the full angle be devided into");
            angleDensityToolTip.Add(Language.Polish, "Określa, na ile jednostek zostanie podzielony kąt pełny");
            angleDensity.Tooltips = angleDensityToolTip;
            OptionTemplates.Add(angleDensity);

            // MAXDIFF
            Dictionary<Language, string> maxDiffName = new Dictionary<Language, string>();
            Dictionary<Language, string> maxDiffToolTip = new Dictionary<Language, string>();
            Option maxDiff = new Option();
            maxDiff.Type = Option.ValueType.Double;
            maxDiff.Value = 0.01d;
            maxDiff.MinValue = 0.001d;
            maxDiff.MaxValue = 1d;
            maxDiffName.Add(Language.English, "Point size");
            maxDiffName.Add(Language.Polish, "Wielkość punktu");
            maxDiffToolTip.Add(Language.English, "Describes, what is the maximum distance between two points for algorithm to act like it is one point");
            maxDiffToolTip.Add(Language.Polish, "Określa maksymalną odległość między punktami, żeby algorytm traktował te punkty jak jeden");
            maxDiff.Names = maxDiffName;
            maxDiff.Tooltips = maxDiffToolTip;
            OptionTemplates.Add(maxDiff);


            // TURNEDGEWEIGHT
            Dictionary<Language, string> turnEdgeWeightName = new Dictionary<Language, string>();
            Dictionary<Language, string> turnEdgeWeightToolTip = new Dictionary<Language, string>();
            Option turnEdgeWeight = new Option();
            turnEdgeWeight.Type = Option.ValueType.Integer;
            turnEdgeWeight.MinValue = 0;
            turnEdgeWeight.MaxValue = 100;
            turnEdgeWeight.Value = 10;
            turnEdgeWeightName.Add(Language.English, "Graph edge weight for turns");
            turnEdgeWeightName.Add(Language.Polish, "Waga krawędzi grafu dla obrotu");
            turnEdgeWeightToolTip.Add(Language.English, "Describes the value of graph edge weight for every unit turn - the bigger the value, the less turns will vehicle take");
            turnEdgeWeightToolTip.Add(Language.Polish, "Określa wagę krawędzi w grafie dla obrotu pojazdu o jedną jednostę - im większa wartość, tym mniej obrotów pojazd wykona");
            turnEdgeWeight.Names = turnEdgeWeightName;
            turnEdgeWeight.Tooltips = turnEdgeWeightToolTip;
            OptionTemplates.Add(turnEdgeWeight);

            // MOVEBACKWARDS
            Dictionary<Language, string> moveBackwardsName = new Dictionary<Language, string>();
            Dictionary<Language, string> moveBackwardsToolTip = new Dictionary<Language, string>();
            Option moveBackwards = new Option();
            moveBackwards.Type = Option.ValueType.Boolean;
            moveBackwards.MaxValue = null;
            moveBackwards.MinValue = null;
            moveBackwards.Value = false;
            moveBackwardsName.Add(Language.English, "Allow reverse");
            moveBackwardsName.Add(Language.Polish, "Zezwalaj na wsteczny");
            moveBackwardsToolTip.Add(Language.English, "If set, allows vehicle to move front and back");
            moveBackwardsToolTip.Add(Language.Polish, "Jeśli zaznaczony, zezwala pojazdowi na poruszanie się do przodu i do tyłu");
            moveBackwards.Names = moveBackwardsName;
            moveBackwards.Tooltips = moveBackwardsToolTip;
            OptionTemplates.Add(moveBackwards);

            // ANYDIRECTION
            Dictionary<Language, string> anyDirectionName = new Dictionary<Language, string>();
            Dictionary<Language, string> anyDirectionToolTip = new Dictionary<Language, string>();
            Option anyDirection = new Option();
            anyDirection.Type = Option.ValueType.Boolean;
            anyDirection.MaxValue = null;
            anyDirection.MinValue = null;
            anyDirection.Value = false;
            anyDirectionName.Add(Language.English, "Allow vehicle to move in any direction");
            anyDirectionName.Add(Language.Polish, "Zezwalaj na poruszanie się we wszystkich kierunkach");
            anyDirectionToolTip.Add(Language.English, "If set, allows vehicle to move in any direction, ignores \"Allow reverse\" option");
            anyDirectionToolTip.Add(Language.Polish, "Jeśli zaznaczony, zezwala pojazdowi na poruszanie się we wszystkich kierunkach, ignoruje opcję \"Zezwalaj na wsteczny\"");
            anyDirection.Names = anyDirectionName;
            anyDirection.Tooltips = anyDirectionToolTip;
            OptionTemplates.Add(anyDirection);


            for (int i = 0; i < OptionTemplates.Count; i++)
            {
                Option o = new Option();
                o.Type = OptionTemplates[i].Type;
                o.MaxValue = OptionTemplates[i].MaxValue;
                o.MinValue = OptionTemplates[i].MinValue;
                o.Value = OptionTemplates[i].Value;
                o.Names = OptionTemplates[i].Names;
                o.Tooltips = OptionTemplates[i].Tooltips;
                CurrentOptions.Add(o);
            }

        }

        public List<Polygon>[] MinkowskiSum(Map map, Vehicle vehicle, int angleDensity)
        {
            List<Polygon>[] tableOfObstacles = new List<Polygon>[angleDensity];
            double singleAngle = 2 * Math.PI / angleDensity;
            List<List<Point>> triangularObstacles = new List<List<Point>>();

            for (int i = 0; i < map.Obstacles.Count; i++)
            {
                List<List<Point>> triangles = Triangulate(map.Obstacles[i].Vertices);
                for (int j = 0; j < triangles.Count; j++)
                {
                    triangularObstacles.Add(triangles[j]);
                }
            }

            for (int i = 0; i < angleDensity; i++)
            {
                List<Point> rotatedVehicle = new List<Point>();
                for (int j = 0; j < vehicle.Shape.Vertices.Count; j++)
                {
                    rotatedVehicle.Add(GeometryHelper.Rotate(vehicle.Shape.Vertices[j], new Point(0, 0), i * singleAngle + Math.PI));
                }

                List<List<Point>> triangularVehicle = Triangulate(rotatedVehicle);
                List<Polygon> newObstacles = new List<Polygon>();
                object locker = new object();
                Parallel.For(0, triangularVehicle.Count, j =>
                {
                    for (int k = 0; k < triangularObstacles.Count; k++)
                    {
                        Polygon poly = ConvexHull(ConvexMinkowski(triangularVehicle[j], triangularObstacles[k]));
                        lock (locker)
                        {
                            newObstacles.Add(poly);

                        }
                    }
                });
                tableOfObstacles[i] = newObstacles;
            }

            for (int i = 0; i < tableOfObstacles.Length; i++)
                tableOfObstacles[i] = GeometryHelper.Union(tableOfObstacles[i]);

            return tableOfObstacles;
        }

        List<Point> ConvexMinkowski(List<Point> polygon1, List<Point> polygon2)
        {
            List<Point> list = new List<Point>();
            for (int i = 0; i < polygon1.Count; i++)
            {
                for (int j = 0; j < polygon2.Count; j++)
                    list.Add(new Point(polygon1[i].X + polygon2[j].X, polygon1[i].Y + polygon2[j].Y));
            }
            return list;
        }

        List<List<Point>> Triangulate(List<Point> shape)
        {
            Polygon poly = new Polygon(shape.ToArray());
            List<Point[]> triangles = Triangulation2D.Triangulate(ref poly);
            List<List<Point>> list = new List<List<Point>>();
            for (int i = 0; i < triangles.Count; i++)
            {
                List<Point> tmpList = new List<Point>();
                for (int j = 0; j < triangles[i].Length; j++)
                {
                    tmpList.Add(triangles[i][j]);
                }
                list.Add(tmpList);
            }
            return list;
        }

        Polygon ConvexHull(List<Point> points)
        {
            Polygon poly;
            points.Sort((a, b) =>
                a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
            List<Point> U = new List<Point>(), L = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                while (L.Count > 1 && GeometryHelper.IsCounterClockwiseTurn(L[L.Count - 2], L[L.Count - 1], points[i]))
                    L.RemoveAt(L.Count - 1);
                L.Add(points[i]);
            }
            for (int i = points.Count - 1; i >= 0; i--)
            {
                while (U.Count > 1 && GeometryHelper.IsCounterClockwiseTurn(U[U.Count - 2], U[U.Count - 1], points[i]))
                    U.RemoveAt(U.Count - 1);
                U.Add(points[i]);
            }
            U.RemoveAt(U.Count - 1);
            L.RemoveAt(L.Count - 1);
            for (int i = 0; i < L.Count; i++)
            {
                U.Add(L[i]);
            }
            poly = new Polygon(U);
            return poly;
        }

        private bool IsPointInTriangle(Point p1, Point p2, double angle, Polygon triangle)
        {
            List<Point> newTriangle = new List<Point>();
            for (int i = 0; i < triangle.Vertices.Count; i++)
            {
                Point p = GeometryHelper.Rotate(triangle.Vertices[i], new Point(0, 0), angle);
                newTriangle.Add(new Point(p1.X + p.X, p1.Y + p.Y));
            }
            Polygon poly = new Polygon(newTriangle);
            return GeometryHelper.IsEnclosed(p2, poly);
        }


        private int GetEdgeWeight(Point p1, Point p2)
        {
            return (int)Math.Round(500 * Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)), 0);
        }

        bool CanTwoPointsConnect(Point p1, Point p2, List<Polygon> obstacles, double angle)
        {
            if (Math.Abs(p1.X) > 1 || Math.Abs(p1.Y) > 1 || Math.Abs(p2.X) > 1 || Math.Abs(p2.Y) > 1)
                return false;
            for (int i = 0; i < obstacles.Count; i++)
            {
                for (int j = 0; j < obstacles[i].Vertices.Count; j++)
                {
                    if (GeometryHelper.DoSegmentsIntersect(p1, p2, obstacles[i].Vertices[j], obstacles[i].Vertices[(j + 1) % obstacles[i].Vertices.Count]))
                    {
                        if (p1 == obstacles[i].Vertices[j] || p2 == obstacles[i].Vertices[j] || p1 == obstacles[i].Vertices[(j + 1) % obstacles[i].Vertices.Count] || p2 == obstacles[i].Vertices[(j + 1) % obstacles[i].Vertices.Count])
                        {
                            continue;
                        }
                        return false;
                    }
                }
                if (GeometryHelper.IsEnclosed(p1, obstacles[i]) && !obstacles[i].Vertices.Contains(p1) || GeometryHelper.IsEnclosed(p2, obstacles[i]) && !obstacles[i].Vertices.Contains(p2))
                    return false;
                if (GeometryHelper.IsEnclosed(new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2), obstacles[i]))
                {
                    for (int index = 0; index < obstacles[i].Vertices.Count; index++)
                    {
                        if (obstacles[i].Vertices[index] == p1)
                        {
                            if (obstacles[i].Vertices[(index + 1) % obstacles[i].Vertices.Count] != p2 && obstacles[i].Vertices[(index - 1 + obstacles[i].Vertices.Count) % obstacles[i].Vertices.Count] != p2)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public List<Option> GetOptions()
        {
            List<Option> options = new List<Option>();
            for (int i = 0; i < OptionTemplates.Count; i++)
            {
                Option o = new Option();
                o.Type = OptionTemplates[i].Type;
                o.MaxValue = OptionTemplates[i].MaxValue;
                o.MinValue = OptionTemplates[i].MinValue;
                o.Value = OptionTemplates[i].Value;
                o.Names = OptionTemplates[i].Names;
                o.Tooltips = OptionTemplates[i].Tooltips;
                options.Add(o);
            }
            return options;
        }

        public void SetOptions(List<Option> options)
        {
            CurrentOptions = options;
        }
    }
}
