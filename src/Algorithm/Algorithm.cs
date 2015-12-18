using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRL.Model.Model;
using SRL.Main.Utilities;

namespace SRL.Model
{
    public class Algorithm : IAlgorithm
    {
        List<Order> IAlgorithm.GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="vehicle">znormalizowany pojazd (punkt (0,0) i kąt 0)</param>
        /// <param name="angleDensity"></param>
        public List<Polygon>[] MinkowskiSum(Map map, Vehicle vehicle, int angleDensity)
        {
            // TODO: normalisation
            List<Point> lst = new List<Point>();
            for(int i=0; i<vehicle.Shape.VertexCount;i++)
            {
                lst.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[i] - vehicle.OrientationOrigin, new Point(0, 0), -vehicle.OrientationAngle));
            }
            vehicle = new Vehicle(new Polygon(lst.ToArray()), new Point(0, 0), 0);
            List<Polygon>[] tableOfObstacles = new List<Polygon>[angleDensity]; // każdy element tablicy to mapa dla danego obrotu, w każdym obrocie mamy listę przeszkód. każda przeszkoda to lista punktów
            double singleAngle = Math.PI / angleDensity;
            List<List<Point>> triangularObstacles = new List<List<Point>>();
            for(int i=0;i<map.ObstacleCount;i++)
            {
                List<List<Point>> triangles = Triangulate(map.Obstacles[i].Vertices);
                for(int j=0;j<triangles.Count;j++)
                {
                    triangularObstacles.Add(triangles[j]);
                }
            }
            for(double i=0;i<Math.PI;i+=singleAngle) // indeks w tablicy = i/singleangle
            {
                List<Point> rotatedVehicle = new List<Point>();
                for(int j=0;j<vehicle.Shape.VertexCount;j++)
                {
                    rotatedVehicle.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[j], vehicle.OrientationOrigin, i + Math.PI));
                }
                List<List<Point>> triangularVehicle = Triangulate(rotatedVehicle);
                List<Polygon> newObstacles = new List<Polygon>();
                for(int j=0;j<triangularVehicle.Count;j++)
                {
                    for(int k=0;k<triangularObstacles.Count;k++)
                    {
                        newObstacles.Add(ConvexHull( ConvexMinkowski(triangularVehicle[j], triangularObstacles[k])));
                    }
                }
                tableOfObstacles[(int)(i / singleAngle)] = newObstacles;
            }
            return tableOfObstacles;
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
