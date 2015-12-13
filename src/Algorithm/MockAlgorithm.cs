using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRL.Model.Model;

namespace SRL.Model
{
    public class MockAlgorithm : IAlgorithm
    {
        public List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity)
        {
            foreach (Point p in vehicle.Shape.Vertices)
            {
                // TODO: Vehicle out of map.
                    //if (p.X >= map.Width || p.X < 0 || p.Y >= map.Height || p.Y < 0)
                    //    throw new Exception("Vehicle out of map");
            }
            List<Point> lst = new List<Point>();
            for (int i = 0; i < vehicle.Shape.VertexCount; i++)
            {
                lst.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[i] - vehicle.OrientationOrigin, new Point(0, 0), -vehicle.OrientationAngle));
            }
            Vehicle vehicleTemplate = new Vehicle(new Polygon(lst), new Point(0, 0), 0);

            return GenerateRandomOrders(map, vehicleTemplate, start, end, vehicleRotation + vehicle.OrientationAngle);
        }

        public List<Order> GenerateRandomOrders(Map map, Vehicle vehicleTemplate, Point start, Point end, double vehicleRotation)
        {
            Random r = new Random();
            List<Order> lst = new List<Order>();
            int iterations = r.Next(5) + 2;
            //int iterations = 0;
            Order o = new Order();
            Vehicle currentState = new Vehicle();
            List<Point> shp = new List<Point>();
            for (int i = 0; i < vehicleTemplate.Shape.VertexCount; i++)
            {
                shp.Add(vehicleTemplate.Shape.Vertices[i] + start);
            }
            currentState = new Vehicle(new Polygon(shp), start, vehicleRotation);
            //o.Rotation = vehicleRotation; o.Destination = start;
            //lst.Add(o);
            for (int i = 0; i < iterations; i++)
            {
                bool c = false;
                double rotation = 0, x = 0, y = 0;
                do
                {
                    c = false;
                    shp.Clear();
                    y = r.Next((int)map.Height);
                    x = r.Next((int)map.Width);
                    try
                    {
                        rotation = Math.Atan((-currentState.OrientationOrigin.Y + y) / (-currentState.OrientationOrigin.X + x));
                    }
                    catch
                    {
                        continue;
                    }
                    
                    if (x < currentState.OrientationOrigin.X)
                        rotation += Math.PI;
                    if (rotation < 0)
                        rotation = rotation + (2 * Math.PI);
                    
                    for (int j = 0; j < vehicleTemplate.Shape.VertexCount; j++)
                    {
                        Point rotatedPoint = GeometryHelper.RotatePoint(vehicleTemplate.Shape.Vertices[j], new Point(0, 0), rotation);
                        double newPointX = x + vehicleTemplate.Shape.Vertices[j].X;
                        double newPointY = y + vehicleTemplate.Shape.Vertices[j].Y;
                        shp.Add(new Point(newPointX, newPointY));
                        if (newPointX >= map.Width || newPointX < 0 || newPointY >= map.Height || newPointY < 0)
                        {
                            c = true;
                            break;
                        }
                    }
                    
                } while (c);
                lst.Add(new Order { Rotation = -rotation, Destination = new Point(x, y) });
                currentState = new Vehicle(new Polygon(shp), new Point(x, y), rotation);
            }
            double finalRotation;
            finalRotation = Math.Atan((-currentState.OrientationOrigin.Y + end.Y) / (-currentState.OrientationOrigin.X + end.X));
            if (currentState.OrientationOrigin.X > end.X)
                finalRotation += Math.PI;
            lst.Add(new Order() { Rotation = -finalRotation, Destination = end });
            return lst;
        }

        
    }
}
