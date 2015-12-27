using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRL.Model.Model;
using SRL.Main.Utilities;
using ASD.Graph;
using ClipperLib;

namespace SRL.Model
{
    public class Algorithm : IAlgorithm
    {
        struct IPoint
        {
            public Point point;
            public int index;
            public int angle;
            public int obstacle;
        }
        List<Order> IAlgorithm.GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity)
        {
            int maxDiff = 15;
            double singleAngle = 2 * Math.PI / angleDensity;
            List<List<IPoint>>[] iPointObstacles = new List<List<IPoint>>[angleDensity];
            map.Obstacles.Add(new Polygon(new Point[] { new Point(0, 0), new Point(0, map.Height), new Point(0, map.Height) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(0, 0), new Point(map.Width, 0), new Point(map.Width, 0) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(map.Width, map.Height), new Point(map.Width, 0), new Point(map.Width, 0) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(map.Width, map.Height), new Point(0, map.Height), new Point(0, map.Height) }));
            List<IPoint> IndexPointAngleList = new List<IPoint>();
            IGraph graph;
            List<Polygon>[] currentMap = MinkowskiSum(map, vehicle, angleDensity);
            int index = 0;
            for (int i = 0; i < angleDensity; i++)
            {
                int vertices = 2;
                for (int j = 0; j < currentMap[i].Count; j++)
                    vertices += currentMap[i][j].Vertices.Count;
                IPoint ip = new IPoint();
                ip.angle = i;
                ip.index = index++;
                ip.point = start;
                ip.obstacle = -1;
                IndexPointAngleList.Add(ip);
                int k = 0;
                foreach (Polygon poly in currentMap[i])
                {
                    foreach (Point p in poly.Vertices)
                    {
                        ip.point = p;
                        ip.index = index++;
                        ip.obstacle = k;
                        IndexPointAngleList.Add(ip);
                    }
                    k++;
                }
                ip.point = end;
                ip.index = index++;
                ip.obstacle = -1;
                IndexPointAngleList.Add(ip);
            }
            graph = new AdjacencyListsGraph<HashTableAdjacencyList>(true, index + 1);
            for (int i = 0; i < IndexPointAngleList.Count; i++)
            {
                for (int j = 0; j < IndexPointAngleList.Count; j++)
                {
                    if (i == j) continue;
                    if (IndexPointAngleList[i].angle > IndexPointAngleList[j].angle) continue;
                    if (IndexPointAngleList[j].angle > IndexPointAngleList[i].angle) break;
                    bool addEdge = true;
                    if (CanTwoPointsConnect(IndexPointAngleList[i].point, IndexPointAngleList[j].point, currentMap[IndexPointAngleList[i].angle]))
                    {
                        if(IndexPointAngleList[i].obstacle == IndexPointAngleList[j].obstacle && IndexPointAngleList[i].obstacle >= 0)
                        {
                            if (GeometryHelper.IsPointInPolygon(new Point((IndexPointAngleList[i].point.X + IndexPointAngleList[j].point.X)/2, (IndexPointAngleList[i].point.Y + IndexPointAngleList[j].point.Y) / 2), currentMap[IndexPointAngleList[i].angle][IndexPointAngleList[i].obstacle]))
                                addEdge = false;
                        }
                        if (addEdge)
                            graph.AddEdge(new Edge(i, j,GetEdgeWeight(IndexPointAngleList[i].point, IndexPointAngleList[j].point)));
                    }
                }
            }
            for (int i = 0; i < IndexPointAngleList.Count; i++)
            {
                for (int j = 0; j < IndexPointAngleList.Count; j++)
                {
                    if ((IndexPointAngleList[i].angle + 1) % angleDensity != IndexPointAngleList[j].angle)
                        continue;
                    if (GetEdgeWeight(IndexPointAngleList[i].point, IndexPointAngleList[j].point) <= maxDiff)
                    {
                        graph.AddEdge(i, j, GetEdgeWeight(IndexPointAngleList[i].point, IndexPointAngleList[j].point) + 1);
                        graph.AddEdge(j, i, GetEdgeWeight(IndexPointAngleList[i].point, IndexPointAngleList[j].point) + 1);
                    }
                }
            }
            for(int i=0;i<IndexPointAngleList.Count;i++)
            {
                if (IndexPointAngleList[i].obstacle == -1 && IndexPointAngleList[i].point == end)
                    graph.AddEdge(i, index , 0);
            }

            Edge[] path;
            AStarGraphExtender.AStar(graph, 0, graph.VerticesCount - 1, out path);
            if (path == null)
                return null;
            List<Order> orders = new List<Order>();

            for (int i = 0; i < path.Length - 1; i++)
            {
                int a = IndexPointAngleList[path[i].From].obstacle;
                int b = IndexPointAngleList[path[i].To].obstacle;
            }
            for (int i=0; i<path.Length-1;i++)
            {
                Order o = new Order();
                o.Destination = IndexPointAngleList[path[i].To].point;
                o.Rotation = IndexPointAngleList[path[i].To].angle * singleAngle;
                orders.Add(o);
            }

            return orders;
            /*
            map.Obstacles.Add(new Polygon(new Point[] { new Point(-1, -1), new Point(-1, 1), new Point(-1, 1) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(-1, -1), new Point(1, -1), new Point(1, -1) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(1, 1), new Point(1, -1), new Point(1, -1) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(1, 1), new Point(-1, 1), new Point(-1, 1) }));
            */
            
        }


        private int GetEdgeWeight(Point p1, Point p2)
        {
            return (int)Math.Round(Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)), 0);
        }
        bool CanTwoPointsConnect(Point p1, Point p2, List<Polygon> obstacles)
        {
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="vehicle">znormalizowany pojazd (punkt (0,0) i kąt 0)</param>
        /// <param name="angleDensity"></param>
        public List<Polygon>[] MinkowskiSum(Map map, Vehicle vehicle, int angleDensity)
        {
            map.Obstacles.Add(new Polygon(new Point[] { new Point(0, 0), new Point(0, map.Height), new Point(1, map.Height), new Point(1, 0) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(0, 0), new Point(map.Width, 0), new Point(map.Width, 1), new Point(0, 1) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(map.Width, map.Height), new Point(map.Width, 0), new Point(map.Width -1, 0), new Point(map.Width -1, map.Height) }));
            map.Obstacles.Add(new Polygon(new Point[] { new Point(map.Width, map.Height), new Point(0, map.Height), new Point(0, map.Height-1), new Point(map.Width, map.Height-1) }));

            List<Point> lst = new List<Point>();
            for(int i=0; i<vehicle.Shape.VertexCount;i++)
            {
                lst.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[i] - vehicle.OrientationOrigin, new Point(0, 0), -vehicle.OrientationAngle));
            }
            vehicle = new Vehicle(new Polygon(lst.ToArray()), new Point(0, 0), 0);
            List<Polygon>[] tableOfObstacles = new List<Polygon>[angleDensity]; // każdy element tablicy to mapa dla danego obrotu, w każdym obrocie mamy listę przeszkód. każda przeszkoda to lista punktów
            double singleAngle = 2 * Math.PI / angleDensity;
            List<List<Point>> triangularObstacles = new List<List<Point>>();
            for(int i=0;i<map.ObstacleCount;i++)
            {
                List<List<Point>> triangles = Triangulate(map.Obstacles[i].Vertices);
                for(int j=0;j<triangles.Count;j++)
                {
                    triangularObstacles.Add(triangles[j]);
                }
            }
            for(int i=0;i<angleDensity;i+=1) // indeks w tablicy = i/singleangle
            {
                List<Point> rotatedVehicle = new List<Point>();
                for(int j=0;j<vehicle.Shape.VertexCount;j++)
                {
                    rotatedVehicle.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[j], vehicle.OrientationOrigin, i*singleAngle + Math.PI));
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
                /*for(int j=0;j<triangularVehicle.Count;j++)
                {
                    for (int k = 0; k < triangularObstacles.Count; k++)
                    {
                        newObstacles.Add(ConvexHull(ConvexMinkowski(triangularVehicle[j], triangularObstacles[k])));
                    }
                }*/
                tableOfObstacles[i] = newObstacles;
                
                
            }
            for(int i=0;i<tableOfObstacles.Length;i++)
            {
                tableOfObstacles[i] = MergePolygons(tableOfObstacles[i]);
            }

            return tableOfObstacles;
        }




        List<Polygon> MergePolygons(List<Polygon> polygons)
        {
            if (polygons == null)
                return null;
            List<Polygon> polys = new List<Polygon>();
            List<List<IntPoint>> clipPolys = new List<List<IntPoint>>();
            foreach(Polygon p in polygons)
            {
                List<IntPoint> tmpPoly = new List<IntPoint>();
                for (int i = 0; i < p.Vertices.Count; i++)
                    tmpPoly.Add(new IntPoint(p.Vertices[i].X, p.Vertices[i].Y));
                clipPolys.Add(tmpPoly);
            }
            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            Clipper c = new Clipper();
            c.AddPaths(clipPolys, PolyType.ptClip, true);
            bool succeeded = c.Execute(ClipType.ctUnion, solution,
                        PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            if(succeeded)
            {
                foreach(List<IntPoint> p in solution)
                {
                    List<Point> tmpPoly = new List<Point>();
                    for (int i = 0; i < p.Count; i++)
                        tmpPoly.Add(new Point(p[i].X, p[i].Y));
                    polys.Add(new Polygon(tmpPoly));
                }
            }
            return polys;
        }

        List<Point> ConvexMinkowski(List<Point> polygon1, List<Point> polygon2)
        {
            
            List<Point> list = new List<Point>();
            for(int i=0;i<polygon1.Count;i++)
            {
                for (int j = 0; j < polygon2.Count; j++)
                    list.Add(polygon1[i] + polygon2[j]);
            }
            return list;
        }

        List<List<Point>> Triangulate(List<Point> shape)
        {
            Polygon poly = new Polygon(shape.ToArray());
            List<Point[]> triangles = Triangulation2D.Triangulate(ref poly);
            List<List<Point>> list = new List<List<Point>>();
            for(int i=0; i<triangles.Count; i++)
            {
                List<Point> tmpList = new List<Point>();
                for(int j=0;j<triangles[i].Length;j++)
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
            for (int i = points.Count-1; i >= 0 ; i--)
            {
                while (U.Count > 1 && GeometryHelper.IsCounterClockwiseTurn(U[U.Count - 2], U[U.Count - 1], points[i]))
                    U.RemoveAt(U.Count - 1);
                U.Add(points[i]);
            }
            U.RemoveAt(U.Count - 1);
            L.RemoveAt(L.Count - 1);
            for(int i=0;i<L.Count;i++)
            {
                U.Add(L[i]);
            }
            poly = new Polygon(U.ToArray());
            return poly;
        }

    }
}
