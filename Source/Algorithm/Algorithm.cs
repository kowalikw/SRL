using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        //TODO fix `==` and Equals() comparisons for doubles throughout the class. Use double.EpsilonEquals() instead.

        //TODO throw OperationCanceledException in meaningful spots (before and after long calculations; MinkowskiSum?). Not just at the beginning of loop iterations.
        
        private List<Option> _currentOptions;
        private readonly List<Option> _defaultOptions; 

        public Algorithm()
        {
            _defaultOptions = GetOptions();
        }

        public List<Option> GetOptions()
        {
            return OptionsFactory();
        }

        public void SetOptions(List<Option> options)
        {
            _currentOptions = options;
        }

        public List<Order> GetPath(Map inputMap, Vehicle inputVehicle, Point start, Point end, double vehicleSize, double vehicleRotation, CancellationToken token)
        {
            if (_currentOptions == null)
                _currentOptions = GetOptions();

            // load user options
            int turnEdgeWeight = (int)LoadOptionValue("turnWeight");
            int moveEdgeWeight = (int)LoadOptionValue("moveWeight");
            double maxDiff = (double)LoadOptionValue("pointPrecision");
            int angleDensity = (int)LoadOptionValue("angleDensity");
            bool backwards = (bool)LoadOptionValue("bidirectional");
            bool allDirections = (bool)LoadOptionValue("multidirectional");


            double singleAngle = 2 * Math.PI / angleDensity;
            // changing size of the vehicle to current one
            List<Point> lst = new List<Point>();
            for (int i = 0; i < inputVehicle.Shape.Vertices.Count; i++)
            {
                lst.Add(new Point(inputVehicle.Shape.Vertices[i].X * vehicleSize, inputVehicle.Shape.Vertices[i].Y * vehicleSize));
            }
            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(lst);

            // copy of the map with additional obstacles as map edges (commented at the moment) (size of those additional obstacles can be changed, it works fine for these ones), right now map is without bounds as we have trouble with vehicles moving by normal obstacles
            Map map = new Map();
            map.Obstacles.Add(new Polygon(new[] { new Point(-1, -1), new Point(-1, 1), new Point(-1.1, 0) }));
            map.Obstacles.Add(new Polygon(new[] { new Point(-1, -1), new Point(1, -1), new Point(0, -1.1) }));
            map.Obstacles.Add(new Polygon(new[] { new Point(1, 1), new Point(1, -1), new Point(1.1, 0) }));
            map.Obstacles.Add(new Polygon(new[] { new Point(1, 1), new Point(-1, 1), new Point(0, 1.1) }));

            List<IndexPoint>[] indexPointAngleList = new List<IndexPoint>[angleDensity];
            for (int i = 0; i < inputMap.Obstacles.Count; i++)
                map.Obstacles.Add(inputMap.Obstacles[i]);

            // Minkowski's sum calculation for each angle
            List<Polygon>[] currentMap = MinkowskiSum(map, vehicle, angleDensity);

            // creating triangle to verify, if points are linear later on
            List<Point> triangleTemplate = new List<Point>();
            triangleTemplate.Add(new Point(0, 0));
            triangleTemplate.Add(new Point(4, 4 * Math.Tan(singleAngle / 2)));
            triangleTemplate.Add(new Point(4, -4 * Math.Tan(singleAngle / 2)));
            Polygon triangle = new Polygon(triangleTemplate);

            // Getting angle for starting set up of the vehicle
            int startingIndex = (int)((vehicleRotation + 2 * Math.PI) % (2 * Math.PI) / singleAngle);


            // setting index for each Minkowski's sum point of each angle
            int index = 0;
            for (int i = 0; i < angleDensity; i++)
            {
                if (i == startingIndex)
                    startingIndex = index; // setting the index of starting point
                indexPointAngleList[i] = new List<IndexPoint>();
                int vertices = 2;
                for (int j = 0; j < currentMap[i].Count; j++)
                    vertices += currentMap[i][j].Vertices.Count;
                IndexPoint ip = new IndexPoint();
                ip.Index = index++;
                ip.Point = start;
                ip.Obstacle = -1;
                indexPointAngleList[i].Add(ip);
                int k = 0;
                foreach (Polygon poly in currentMap[i])
                {
                    foreach (Point p in poly.Vertices)
                    {
                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException();
                        ip.Point = p;
                        ip.Index = index++;
                        ip.Obstacle = k;
                        indexPointAngleList[i].Add(ip);
                    }
                    k++;
                }
                ip.Point = end;
                ip.Index = index++;
                ip.Obstacle = -1;
                indexPointAngleList[i].Add(ip);
            }
            int insides = 0;
            for(int angle = 0;angle<angleDensity;angle++)
            {
                foreach(Polygon obstacle in currentMap[angle])
                    if(GeometryHelper.IsEnclosed(end,obstacle))
                    {
                        insides++;
                        break;
                    }
                if (insides < angle)
                    break;
                if (angle == angleDensity - 1 && insides == angleDensity)
                    throw new NonexistentPathException();
            }

            // creating a graph with vertices count equal to number of all indexed points enlarged by one (the accepting state for A* algorithm)
            IGraph graph = new AdjacencyListsGraph<HashTableAdjacencyList>(true, index + 1);

            // creating graph edges for each angle (all done in one graph from the very beginning)
            object locker = new object();
            Parallel.For(0, angleDensity, angle =>
              {
                  for (int i = 0; i < indexPointAngleList[angle].Count; i++)
                  {
                      for (int j = 0; j < indexPointAngleList[angle].Count; j++)
                      {
                          if (token.IsCancellationRequested)
                              return;
                          // Chcecking if starting and ending points for certain angle are not in that Minkowski's sum obstacles
                          /*if (indexPointAngleList[angle][i].Obstacle == -1)
                          {
                              bool cancel = false;
                              foreach (Polygon obstacle in currentMap[angle])
                              {
                                  if (GeometryHelper.IsEnclosed(indexPointAngleList[angle][i].Point, obstacle))
                                  {
                                      cancel = true;
                                      break;
                                  }
                              }
                              if (cancel)
                                  continue;
                          }
                          if (indexPointAngleList[angle][j].Obstacle == -1)
                          {
                              bool cancel = false;
                              foreach (Polygon obstacle in currentMap[angle])
                              {
                                  if (GeometryHelper.IsEnclosed(indexPointAngleList[angle][j].Point, obstacle))
                                  {
                                      cancel = true;
                                      break;
                                  }
                              }
                              if (cancel)
                                  continue;
                          }*/
                          if (i == j) continue; // We are not accepting edges in one point when not turning

                          if (CanTwoPointsConnect(indexPointAngleList[angle][i].Point, indexPointAngleList[angle][j].Point, currentMap[angle], angle * singleAngle))
                          {
                              int weight = GetEdgeWeight(indexPointAngleList[angle][i].Point, indexPointAngleList[angle][j].Point, maxDiff, moveEdgeWeight);
                              if (!allDirections)
                              {
                                  // if the Point that we are going to moce to is inside the triangle turned by the current angle, we can add an edge
                                  if (IsPointInTriangle(indexPointAngleList[angle][i].Point, indexPointAngleList[angle][j].Point, angle * singleAngle, triangle))
                                  {
                                      graph.AddEdge(new Edge(indexPointAngleList[angle][i].Index, indexPointAngleList[angle][j].Index, weight));
                                      // if user enabled reverse in options, we add an edge back
                                      if (backwards)
                                          graph.AddEdge(new Edge(indexPointAngleList[angle][j].Index, indexPointAngleList[angle][i].Index, weight));
                                  }
                              }
                              // if user enabled all directions, we add all edges that passed all previous tests
                              else
                                  graph.AddEdge(new Edge(indexPointAngleList[angle][i].Index, indexPointAngleList[angle][j].Index, weight));
                          }
                      }
                  }
              });

            if (token.IsCancellationRequested)
                throw new OperationCanceledException();

            // Adding turning edges
            for (int angle = 0; angle < angleDensity; angle++)
            {
                for (int i = 0; i < indexPointAngleList[angle].Count; i++)
                {
                    for (int j = 0; j < indexPointAngleList[(angle + 1) % angleDensity].Count; j++)
                    {
                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException();
                        // Again, checking if starting and ending point are not in any Minkowski's sum polygons
                        
                        bool cancel = false;
                        foreach (Polygon obstacle in currentMap[angle])
                        {
                            if (GeometryHelper.IsEnclosed(indexPointAngleList[angle][i].Point, obstacle) && !obstacle.Vertices.Contains(indexPointAngleList[angle][i].Point))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        if (cancel)
                            continue;
                        foreach (Polygon obstacle in currentMap[(angle + 1) % angleDensity])
                        {
                            if (GeometryHelper.IsEnclosed(indexPointAngleList[(angle + 1) % angleDensity][j].Point, obstacle) && !obstacle.Vertices.Contains(indexPointAngleList[(angle + 1) % angleDensity][j].Point))
                            {
                                cancel = true;
                                break;
                            }
                        }
                        if (cancel)
                            continue;
                        if (GeometryHelper.GetDistance(indexPointAngleList[angle][i].Point, indexPointAngleList[(angle + 1) % angleDensity][j].Point) <= maxDiff)
                        {
                            graph.AddEdge(indexPointAngleList[angle][i].Index, indexPointAngleList[(angle + 1) % angleDensity][j].Index, turnEdgeWeight);
                            graph.AddEdge(indexPointAngleList[(angle + 1) % angleDensity][j].Index, indexPointAngleList[angle][i].Index, turnEdgeWeight);
                        }
                    }
                }
            }

            // Adding edges to accepting state
            for (int i = 0; i < indexPointAngleList.Length; i++)
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException();

                if (indexPointAngleList[i][indexPointAngleList[i].Count - 1].Obstacle == -1 && indexPointAngleList[i][indexPointAngleList[i].Count - 1].Point == end)
                    graph.AddEdge(indexPointAngleList[i][indexPointAngleList[i].Count - 1].Index, index, 0);
            }

            // Graph algorithm A*
            Edge[] path;
            AStarGraphExtender.AStar(graph, startingIndex, graph.VerticesCount - 1, out path);

            if (path == null)
                throw new NonexistentPathException();

            // Creating Orders from A* results
            // TODO: still some angle troubles
            List<Order> orders = new List<Order>();
            orders.Add(new Order((vehicleRotation + 2 * Math.PI) % (2 * Math.PI), start)); //TODO a friendly reminder that 0 deg rotation DOES NOT equal -360 deg (the former is CCW)
            for (int i = 0; i < path.Length - 1; i++)
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException();

                int angle = 0;
                while (path[i].To > indexPointAngleList[angle][indexPointAngleList[angle].Count - 1].Index)
                    angle++;
                int ind = 0;
                while (indexPointAngleList[angle][ind].Index != path[i].To)
                    ind++;
                Order o = new Order(angle * singleAngle, indexPointAngleList[angle][ind].Point);

                if ((o.Rotation + 2 * Math.PI) % (2 * Math.PI) == (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI))
                {
                    orders.Add(new Order(orders[orders.Count - 1].Rotation, o.Destination));
                    continue;
                }

                if (o.Rotation == 0 && orders[orders.Count - 1].Rotation <= -Math.PI)
                {
                    o = new Order(-2 * Math.PI, o.Destination);
                }
                else if (o.Rotation == 0 && orders[orders.Count - 1].Rotation > 0)
                {
                    // do nothing
                }
                else if (o.Rotation > 0 && orders[orders.Count - 1].Rotation > 0)
                {
                    if (o.Rotation < orders[orders.Count - 1].Rotation)
                        o = new Order(o.Rotation - 2 * Math.PI, o.Destination);
                    // else do nothing
                }
                else if (o.Rotation > 0 && orders[orders.Count - 1].Rotation < 0)
                {
                    if ((o.Rotation + 2 * Math.PI) % (2 * Math.PI) < (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI))
                        o = new Order(o.Rotation - 2 * Math.PI, o.Destination);
                    else if (orders[orders.Count - 1].Rotation == -2 * Math.PI && o.Rotation > Math.PI)
                        o = new Order(o.Rotation - 2 * Math.PI, o.Destination);
                    // else do nothing
                }
                else if (o.Rotation > 0 && orders[orders.Count - 1].Rotation == 0)
                {
                    if (o.Rotation > Math.PI)
                        o = new Order(o.Rotation - 2 * Math.PI, o.Destination);
                }

                orders.Add(o);
            }

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


        private List<Polygon>[] MinkowskiSum(Map map, Vehicle vehicle, int angleDensity)
        {
            List<Polygon>[] tableOfObstacles = new List<Polygon>[angleDensity];
            double singleAngle = 2 * Math.PI / angleDensity;
            List<List<List<Point>>> triangularObstacles = new List<List<List<Point>>>();
            for (int i = 0; i < map.Obstacles.Count; i++)
            {
                triangularObstacles.Add(Triangulate(map.Obstacles[i].Vertices));
            }
            for (int i = 0; i < angleDensity; i++)
            {
                tableOfObstacles[i] = new List<Polygon>();
                Polygon rotatedVehicle = new Polygon();
                List<List<Point>> triangularVehicle = new List<List<Point>>();
                for (int j = 0; j < vehicle.Shape.Vertices.Count; j++)
                {
                    rotatedVehicle = GeometryHelper.Rotate(vehicle.Shape, Math.PI + i * singleAngle);
                }
                triangularVehicle = Triangulate(rotatedVehicle.Vertices);

                foreach (List<List<Point>> obstacle in triangularObstacles)
                {
                    List<Polygon> convexSubPolygons = new List<Polygon>();
                    foreach (List<Point> VehicleTriangle in triangularVehicle)
                    {
                        foreach (List<Point> obstacleTriangle in obstacle)
                        {
                            convexSubPolygons.Add(ConvexHull(ConvexMinkowski(VehicleTriangle, obstacleTriangle)));
                        }
                    }
                    List<Polygon> lst = GeometryHelper.Union(convexSubPolygons);
                    foreach (Polygon poly in lst)
                        tableOfObstacles[i].Add(poly);
                }
            }
            return tableOfObstacles;
        }

        private List<Point> ConvexMinkowski(List<Point> polygon1, List<Point> polygon2)
        {
            List<Point> list = new List<Point>();
            for (int i = 0; i < polygon1.Count; i++)
            {
                for (int j = 0; j < polygon2.Count; j++)
                    list.Add(new Point(polygon1[i].X + polygon2[j].X, polygon1[i].Y + polygon2[j].Y));
            }
            return list;
        }

        private List<List<Point>> Triangulate(List<Point> shape)
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

        private Polygon ConvexHull(List<Point> points)
        {
            Polygon poly;
            points.Sort((a, b) =>
                a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
            List<Point> u = new List<Point>(), l = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                while (l.Count > 1 && GeometryHelper.IsCounterClockwiseTurn(l[l.Count - 2], l[l.Count - 1], points[i]))
                    l.RemoveAt(l.Count - 1);
                l.Add(points[i]);
            }
            for (int i = points.Count - 1; i >= 0; i--)
            {
                while (u.Count > 1 && GeometryHelper.IsCounterClockwiseTurn(u[u.Count - 2], u[u.Count - 1], points[i]))
                    u.RemoveAt(u.Count - 1);
                u.Add(points[i]);
            }
            u.RemoveAt(u.Count - 1);
            l.RemoveAt(l.Count - 1);
            for (int i = 0; i < l.Count; i++)
            {
                u.Add(l[i]);
            }
            poly = new Polygon(u);
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


        private int GetEdgeWeight(Point p1, Point p2, double pointSize, double edgeWeight)
        {
            return (int)Math.Round((edgeWeight * GeometryHelper.GetDistance(p1, p2)) / pointSize, 0);
        }

        private bool CanTwoPointsConnect(Point p1, Point p2, List<Polygon> obstacles, double angle)
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


        private static List<Option> OptionsFactory()
        {
            var options = new List<Option>();
            var rm = Localization.Algorithm.ResourceManager;

            
            Option pointPrecision = new Option(Option.ValueType.Double, nameof(pointPrecision))
            {
                Value = 0.01d,
                MinValue = 0.001d,
                MaxValue = 0.2d
            };
            foreach (var l in Enum.GetValues(typeof(Language)))
            {
                Language language = (Language)l;
                CultureInfo culture = language.GetCultureInfo();

                string name = rm.GetString(pointPrecision.Key, culture);
                string tooltip = rm.GetString(pointPrecision.Key + "Tooltip", culture);

                pointPrecision.Names.Add(language, name);
                pointPrecision.Tooltips.Add(language, tooltip);
            }


            Option angleDensity = new Option(Option.ValueType.Integer, nameof(angleDensity))
            {
                Value = 360,
                MinValue = 3,
                MaxValue = null
            };
            foreach (var l in Enum.GetValues(typeof(Language)))
            {
                Language language = (Language)l;
                CultureInfo culture = language.GetCultureInfo();

                string name = rm.GetString(angleDensity.Key, culture);
                string tooltip = rm.GetString(angleDensity.Key + "Tooltip", culture);

                angleDensity.Names.Add(language, name);
                angleDensity.Tooltips.Add(language, tooltip);
            }


            Option moveWeight = new Option(Option.ValueType.Integer, nameof(moveWeight))
            {
                Value = 100,
                MinValue = 1,
                MaxValue = 1000
            };
            foreach (var l in Enum.GetValues(typeof(Language)))
            {
                Language language = (Language)l;
                CultureInfo culture = language.GetCultureInfo();

                string name = rm.GetString(moveWeight.Key, culture);
                string tooltip = rm.GetString(moveWeight.Key + "Tooltip", culture);

                moveWeight.Names.Add(language, name);
                moveWeight.Tooltips.Add(language, string.Format(tooltip, pointPrecision.Names[language]));
            }


            Option turnWeight = new Option(Option.ValueType.Integer, nameof(turnWeight))
            {
                Value = 100,
                MinValue = 1,
                MaxValue = 1000
            };
            foreach (var l in Enum.GetValues(typeof(Language)))
            {
                Language language = (Language)l;
                CultureInfo culture = language.GetCultureInfo();

                string name = rm.GetString(turnWeight.Key, culture);
                string tooltip = rm.GetString(turnWeight.Key + "Tooltip", culture);

                turnWeight.Names.Add(language, name);
                turnWeight.Tooltips.Add(language, string.Format(tooltip, angleDensity.Names[language]));
            }


            Option bidirectional = new Option(Option.ValueType.Boolean, nameof(bidirectional))
            {
                MaxValue = null,
                MinValue = null,
                Value = false
            };
            foreach (var l in Enum.GetValues(typeof(Language)))
            {
                Language language = (Language)l;
                CultureInfo culture = language.GetCultureInfo();

                string name = rm.GetString(bidirectional.Key, culture);
                string tooltip = rm.GetString(bidirectional.Key + "Tooltip", culture);

                bidirectional.Names.Add(language, name);
                bidirectional.Tooltips.Add(language, tooltip);
            }

            
            Option multidirectional = new Option(Option.ValueType.Boolean, nameof(multidirectional))
            {
                Value = false,
                MinValue = null,
                MaxValue = null
            };
            foreach (var l in Enum.GetValues(typeof(Language)))
            {
                Language language = (Language)l;
                CultureInfo culture = language.GetCultureInfo();

                string name = rm.GetString(multidirectional.Key, culture);
                string tooltip = rm.GetString(multidirectional.Key + "Tooltip", culture);

                multidirectional.Names.Add(language, name);
                multidirectional.Tooltips.Add(language, string.Format(tooltip, bidirectional.Names[language]));
            }
            

            // The order in which the options appear to the user.
            options.Add(pointPrecision);
            options.Add(angleDensity);
            options.Add(moveWeight);
            options.Add(turnWeight);
            options.Add(bidirectional);
            options.Add(multidirectional);

            return options;
        }

        private object LoadOptionValue(string key)
        {
            Option option = _currentOptions.First(o => o.Key == key);

            if (option == null || !option.IsValid)
            {
                option = _defaultOptions.First(o => o.Key == key);
            }

            return option.Value;
        }

        private struct IndexPoint // structure used to unify different index with each point
        {
            public Point Point;
            public int Index;
            public int Obstacle;
        }
    }
}
