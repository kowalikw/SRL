using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRL.Commons.Utilities;
using SRL.Commons.Model;
using System.Windows;
using ASD.Graph;
using ClipperLib;
using SRL.Commons.Model.Base;

namespace SRL.Algorithm
{
    public class Algorithm : IAlgorithm
    {

        struct IPoint
        {
            public Point point;
            public int index;
            public int obstacle;
        }
        public List<Order> GetPath(Map InputMap, Vehicle InputVehicle, Point start, Point end, double vehicleSize, double vehicleRotation, int angleDensity)
        {
            List<Point> lst = new List<Point>();
            for (int i = 0; i < InputVehicle.Shape.Vertices.Count; i++)
            {
                lst.Add(new Point(InputVehicle.Shape.Vertices[i].X * vehicleSize, InputVehicle.Shape.Vertices[i].Y * vehicleSize));
            }
            Vehicle vehicle = new Vehicle();
            vehicle.Shape = new Polygon(lst);
            Map map = new Map();
            double maxDiff = 0.01;
            double singleAngle = 2 * Math.PI / angleDensity;
            List<List<IPoint>>[] iPointObstacles = new List<List<IPoint>>[angleDensity];
            /*map.Obstacles.Add(new Polygon(new Point[] { new Point(-1, -1), new Point(-1, 1), new Point(-2, 1), new Point(-2, -1) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(-1, -1), new Point(1, -1), new Point(1, -2), new Point(-1, -2) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(1, 1), new Point(1, -1), new Point(2, -1), new Point(2, 1) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(1, 1), new Point(-1, 1), new Point(-1, 2), new Point(1, 2) }));
            */List<IPoint>[] IndexPointAngleList = new List<IPoint>[angleDensity];
            IGraph graph;
            List<Point> triangleTemplate = new List<Point>();
            for (int i = 0; i < InputMap.Obstacles.Count; i++)
                map.Obstacles.Add(InputMap.Obstacles[i]);
            List<Polygon>[] currentMap = MinkowskiSum(map, vehicle, angleDensity);
            int index = 0;
            triangleTemplate.Add(new Point(0, 0));
            triangleTemplate.Add(new Point(4, 4 * Math.Tan(singleAngle / 2)));
            triangleTemplate.Add(new Point(4, -4 * Math.Tan(singleAngle / 2)));
            Polygon triangle = new Polygon(triangleTemplate);
            for (int i = 0; i < angleDensity; i++)
            {
                IndexPointAngleList[i] = new List<IPoint>();
                int vertices = 2;
                for (int j = 0; j < currentMap[i].Count; j++)
                    vertices += currentMap[i][j].Vertices.Count;
                IPoint ip = new IPoint();
                ip.index = index++;
                ip.point = start;
                ip.obstacle = -1;
                IndexPointAngleList[i].Add(ip);
                /*for (int k=0;k<currentMap[i].Count;k++)
                {
                    foreach (Point p in currentMap[i][k].Vertices)
                    {
                        ip.point = p;
                        ip.index = index++;
                        ip.obstacle = k;
                        IndexPointAngleList[i].Add(ip);
                    }
                }*/
                int k = 0;
                foreach (Polygon poly in currentMap[i])
                {
                    foreach (Point p in poly.Vertices)
                    {
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
            graph = new AdjacencyListsGraph<HashTableAdjacencyList>(true, index + 1);
            for (int angle = 0; angle < angleDensity; angle++)
            {
                for (int i = 0; i < IndexPointAngleList[angle].Count; i++)
                {
                    for (int j = 0; j < IndexPointAngleList[angle].Count; j++)
                    {
                        if (i == j) continue;
                        bool addEdge = true;
                        if (CanTwoPointsConnect(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point, currentMap[angle]))
                        {
                            /*if (IndexPointAngleList[angle][i].obstacle == IndexPointAngleList[angle][j].obstacle && IndexPointAngleList[angle][i].obstacle >= 0)
                            {
                                if (GeometryHelper.IsInsidePolygon(new Point((IndexPointAngleList[angle][i].point.X + IndexPointAngleList[angle][j].point.X) / 2, (IndexPointAngleList[angle][i].point.Y + IndexPointAngleList[angle][j].point.Y) / 2), currentMap[angle][IndexPointAngleList[angle][i].obstacle]))
                                    addEdge = false;
                            }*/
                            if (IndexPointAngleList[angle][i].obstacle == IndexPointAngleList[angle][j].obstacle && IndexPointAngleList[angle][i].obstacle >= 0)
                            {
                                for (int obstacle = 0; obstacle < currentMap[angle].Count; obstacle++)
                                    if (currentMap[angle][obstacle].Vertices.Contains(IndexPointAngleList[angle][i].point))
                                    {
                                        for (int k = 0; k < currentMap[angle][obstacle].Vertices.Count; k++)
                                        {
                                            if (currentMap[angle][obstacle].Vertices[k] == IndexPointAngleList[angle][i].point)
                                                if (currentMap[angle][obstacle].Vertices[(k + 1) % currentMap[angle][obstacle].Vertices.Count] == IndexPointAngleList[angle][j].point || currentMap[angle][obstacle].Vertices[(k - 1 + currentMap[angle][obstacle].Vertices.Count) % currentMap[angle][obstacle].Vertices.Count] == IndexPointAngleList[angle][j].point)
                                                    break;
                                        }
                                        if (GeometryHelper.IsInsidePolygon(new Point((IndexPointAngleList[angle][i].point.X + IndexPointAngleList[angle][j].point.X) / 2, (IndexPointAngleList[angle][i].point.Y + IndexPointAngleList[angle][j].point.Y) / 2), currentMap[angle][obstacle]))
                                        {
                                            addEdge = false;
                                            break;
                                        }
                                    }
                            }
                            if (addEdge)
                            {
                                if (IsPointInTriangle(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point, angle * singleAngle, triangle))
                                    graph.AddEdge(new Edge(IndexPointAngleList[angle][i].index, IndexPointAngleList[angle][j].index, GetEdgeWeight(IndexPointAngleList[angle][i].point, IndexPointAngleList[angle][j].point)));
                            }
                        }
                    }
                }
            }
            for (int angle = 0; angle < angleDensity; angle++)
            {
                for (int i = 0; i < IndexPointAngleList[angle].Count; i++)
                {
                    for (int j = 0; j < IndexPointAngleList[(angle + 1) % angleDensity].Count; j++)
                    {
                        if (GeometryHelper.GetDistance(IndexPointAngleList[angle][i].point, IndexPointAngleList[(angle + 1) % angleDensity][j].point) <= maxDiff)
                        {
                            graph.AddEdge(IndexPointAngleList[angle][i].index, IndexPointAngleList[(angle + 1) % angleDensity][j].index, GetEdgeWeight(IndexPointAngleList[angle][i].point, IndexPointAngleList[(angle + 1) % angleDensity][j].point) + 50);
                            graph.AddEdge(IndexPointAngleList[(angle + 1) % angleDensity][j].index, IndexPointAngleList[angle][i].index, GetEdgeWeight(IndexPointAngleList[angle][i].point, IndexPointAngleList[(angle + 1) % angleDensity][j].point) + 50);
                        }
                    }
                }
            }
            for (int i = 0; i < IndexPointAngleList.Length; i++)
            {
                if (IndexPointAngleList[i][IndexPointAngleList[i].Count - 1].obstacle == -1 && IndexPointAngleList[i][IndexPointAngleList[i].Count - 1].point == end)
                    graph.AddEdge(IndexPointAngleList[i][IndexPointAngleList[i].Count - 1].index, index, 0);
            }

            Edge[] path;
            AStarGraphExtender.AStar(graph, 0, graph.VerticesCount - 1, out path);
            if (path == null)
                return null;
            List<Order> orders = new List<Order>();
            orders.Add(new Order() { Destination = start, Rotation = vehicleRotation});
            for (int i = 0; i < path.Length - 1; i++)
            {
                int angle = 0;
                while (path[i].To > IndexPointAngleList[angle][IndexPointAngleList[angle].Count - 1].index)
                    angle++;
                int ind = 0;
                while (IndexPointAngleList[angle][ind].index != path[i].To)
                    ind++;
                Order o = new Order();
                o.Destination = IndexPointAngleList[angle][ind].point;
                o.Rotation = angle * singleAngle;

                if(o.Rotation == (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI))
                {
                    o.Rotation = (orders[orders.Count - 1].Rotation);
                    orders.Add(o);
                    continue;
                }

                if ((o.Rotation < (orders[orders.Count - 1].Rotation + 2 * Math.PI) % (2 * Math.PI) || (o.Rotation > Math.Abs(orders[orders.Count - 1].Rotation + (2 * Math.PI)) % (2 * Math.PI) && orders[orders.Count - 1].Rotation == 0)))
                    o.Rotation -= (2 * Math.PI);
                
                orders.Add(o);
            }
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


        public List<Polygon>[] MinkowskiSum(Map map, Vehicle vehicle, int angleDensity)
        {
            List<Polygon>[] tableOfObstacles = new List<Polygon>[angleDensity]; // każdy element tablicy to mapa dla danego obrotu, w każdym obrocie mamy listę przeszkód. każda przeszkoda to lista punktów
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
                    rotatedVehicle.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[j], new Point(0, 0), i * singleAngle + Math.PI));
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
            {
                tableOfObstacles[i] = MergePolygons(tableOfObstacles[i]);
            }

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
            poly = new Polygon(U.ToArray());
            return poly;
        }

        private bool IsPointInTriangle(Point p1, Point p2, double angle, Polygon triangle)
        {
            List<Point> newTriangle = new List<Point>();
            for (int i = 0; i < triangle.Vertices.Count; i++)
            {
                Point p = GeometryHelper.RotatePoint(triangle.Vertices[i], new Point(0, 0), angle);
                newTriangle.Add(new Point(p1.X + p.X, p1.Y + p.Y));
            }
            Polygon poly = new Polygon(newTriangle);
            return GeometryHelper.IsInsidePolygon(p2, poly);
        }


        private int GetEdgeWeight(Point p1, Point p2)
        {
            return (int)Math.Round(500 * Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)), 0);
        }

        bool CanTwoPointsConnect(Point p1, Point p2, List<Polygon> obstacles)
        {
            if (Math.Abs(p1.X) > 1 || Math.Abs(p1.Y) > 1 || Math.Abs(p2.X) > 1 || Math.Abs(p2.Y) > 1)
                return false;
            foreach (Polygon obstacle in obstacles)
            {
                for (int i = 0; i < obstacle.Vertices.Count; i++)
                    if (GeometryHelper.DoSegmentsIntersect(p1, p2, obstacle.Vertices[i], obstacle.Vertices[(i + 1) % obstacle.Vertices.Count]))
                    {
                        if (p1 == obstacle.Vertices[i] || p2 == obstacle.Vertices[i] || p1 == obstacle.Vertices[(i + 1) % obstacle.Vertices.Count] || p2 == obstacle.Vertices[(i + 1) % obstacle.Vertices.Count])
                            continue;
                        return false;
                    }
            }
            return true;
        }
        List<Polygon> MergePolygons(List<Polygon> polygons)
        {
            long multiply = long.MaxValue / 10;
            if (polygons == null)
                return null;
            List<Polygon> polys = new List<Polygon>();
            List<List<IntPoint>> clipPolys = new List<List<IntPoint>>();
            foreach (Polygon p in polygons)
            {
                List<IntPoint> tmpPoly = new List<IntPoint>();
                for (int i = 0; i < p.Vertices.Count; i++)
                    tmpPoly.Add(new IntPoint(p.Vertices[i].X * multiply, p.Vertices[i].Y * multiply));
                clipPolys.Add(tmpPoly);
            }
            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            Clipper c = new Clipper();
            c.AddPaths(clipPolys, PolyType.ptClip, true);
            bool succeeded = c.Execute(ClipType.ctUnion, solution,
                        PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            if (succeeded)
            {
                foreach (List<IntPoint> p in solution)
                {
                    List<Point> tmpPoly = new List<Point>();
                    for (int i = 0; i < p.Count; i++)
                        tmpPoly.Add(new Point((double)p[i].X / (double)multiply, (double)p[i].Y / (double)multiply));
                    polys.Add(new Polygon(tmpPoly));
                }
            }
            return polys;
        }


    }
}