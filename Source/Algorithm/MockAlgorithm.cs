using System;
using System.Collections.Generic;
using System.Windows;
using SRL.Commons.Model;
using SRL.Commons.Model.Base;
using SRL.Commons.Utilities;

namespace SRL.Algorithm
{
    public class MockAlgorithm : IAlgorithm
    {
        List<Order> IAlgorithm.GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity)
        {
            foreach (Point p in vehicle.Shape.Vertices)
            {
                // TODO: Vehicle out of map.
                //if (p.X >= map.Width || p.X < 0 || p.Y >= map.Height || p.Y < 0)
                //    throw new Exception("Vehicle out of map");
            }
            List<Point> lst = new List<Point>();

            return GenerateRandomOrders(map, vehicle, start, end, vehicleRotation);
        }

        public List<Order> GenerateRandomOrders(Map map, Vehicle vehicleTemplate, Point start, Point end, double vehicleRotation)
        {
            Point OrientationOrigin = start;
            Random r = new Random();
            List<Order> lst = new List<Order>();
            int iterations = r.Next(5) + 2;
            Order o = new Order();
            Vehicle currentState = new Vehicle();
            List<Point> shp = new List<Point>();
            for (int i = 0; i < vehicleTemplate.Shape.Vertices.Count; i++)
            {
                shp.Add(new Point(vehicleTemplate.Shape.Vertices[i].X + start.X, vehicleTemplate.Shape.Vertices[i].Y + start.Y));
            }
            currentState.Shape = new Polygon(shp);
            for (int i = 0; i < iterations; i++)
            {
                bool c = false;
                double rotation = 0, x = 0, y = 0;
                do
                {
                    c = false;
                    shp.Clear();
                    y = r.NextDouble() * 2 - 1;
                    x = r.NextDouble() * 2 - 1;
                    if (-OrientationOrigin.X + x == 0)
                    {
                        if (y > OrientationOrigin.Y)
                        {
                            rotation = Math.PI / 4;
                        }
                        else
                        {
                            rotation = 3 * Math.PI / 4;
                        }
                    }
                    else
                    {
                        rotation = Math.Atan((-OrientationOrigin.Y + y) / (-OrientationOrigin.X + x));
                    }

                    if (x < OrientationOrigin.X)
                        rotation += Math.PI;
                    if (rotation < 0)
                        rotation = rotation + (2 * Math.PI);

                    for (int j = 0; j < vehicleTemplate.Shape.Vertices.Count; j++)
                    {
                        Point rotatedPoint = GeometryHelper.RotatePoint(vehicleTemplate.Shape.Vertices[j], new Point(0, 0), rotation);
                        double newPointX = x + vehicleTemplate.Shape.Vertices[j].X;
                        double newPointY = y + vehicleTemplate.Shape.Vertices[j].Y;
                        shp.Add(new Point(newPointX, newPointY));
                        if (newPointX >= 1 || newPointX < -1 || newPointY >= 1 || newPointY < -1)
                        {
                            c = true;
                            break;
                        }
                    }

                } while (c);
                lst.Add(new Order { Rotation = -rotation, Destination = new Point(x, y) });
                currentState.Shape = new Polygon(shp);
            }
            double finalRotation;
            if (OrientationOrigin.X == end.X)
            {
                if (end.Y > OrientationOrigin.Y)
                {
                    finalRotation = Math.PI / 4;
                }
                else
                {
                    finalRotation = 3 * Math.PI / 4;
                }
            }
            else
                finalRotation = Math.Atan((-OrientationOrigin.Y + end.Y) / (-OrientationOrigin.X + end.X));
            if (OrientationOrigin.X > end.X)
                finalRotation += Math.PI;
            lst.Add(new Order() { Rotation = -finalRotation, Destination = end });
            return lst;
        }
    }
}
