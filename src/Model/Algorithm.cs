using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRL.Model.Model;

namespace SRL.Model
{
    class Algorithm : IAlgorithm
    {
        List<Order> IAlgorithm.GetPath(Map map, Vehicle vehicle, Point start, Point end, Point vehicleRotation)
        {
            foreach(Point p in vehicle.Shape.Vertices)
            {
                if (p.X >= map.Width || p.X < 0 || p.Y >= map.Height || p.Y < 0)
                    throw new Exception("Vehicle out of map");
            }
            List<Point> lst = new List<Point>();
            for(int i=0;i<vehicle.Shape.VertexCount;i++)
            {
                lst.Add(RotatePoint(vehicle.Shape.Vertices[i] - vehicle.OrientationOrigin,new Point(0,0),-vehicle.OrientationAngle));
            }
            Vehicle vehicleTemplate = new Vehicle(new Polygon(lst),new Point(0,0),0);

            return GenerateRandomOrders(map, vehicleTemplate, start, end);
        }

        List<Order> GenerateRandomOrders(Map map, Vehicle vehicleTemplate, Point start, Point end)
        {
            Random r = new Random();
            List<Order> lst = new List<Order>();
            int iterations = r.Next(10) + 5;
            Order o = new Order();
            Vehicle currentState = new Vehicle();
            List<Point> shp = new List<Point>();
            for(int i=0;i<vehicleTemplate.Shape.VertexCount;i++)
            {
                shp.Add(vehicleTemplate.Shape.Vertices[i] + start);
            }
            currentState = new Vehicle(new Polygon(shp), start, 0);
            o.Rotation = 0; o.Stride = 0;
            lst.Add(o);
            for(int i=0;i<iterations;i++)
            {
                bool c = false;
                double rotation, stride;
                do
                {
                    double strideX = 0, strideY = 0;
                    shp.Clear();
                    rotation = r.NextDouble() * 2 * Math.PI;
                    stride = r.NextDouble() * 200;
                    for(int j=0;j<vehicleTemplate.Shape.VertexCount;j++)
                    {
                        Point rotatedPoint = RotatePoint(vehicleTemplate.Shape.Vertices[j], new Point(0, 0), rotation);
                        strideX = Math.Sin(rotation) * stride;
                        strideY = Math.Cos(rotation) * stride;
                        double newPointX = rotatedPoint.X + strideX + currentState.OrientationOrigin.X;
                        double newPointY = rotatedPoint.Y + strideY + currentState.OrientationOrigin.Y;
                        shp.Add(new Point(newPointX, newPointY));
                        if (newPointX >= map.Width || newPointX < 0 || newPointY >= map.Height || newPointY < 0)
                            c = true;
                    }
                    currentState = new Vehicle(new Polygon(shp), new Point(currentState.OrientationOrigin.X + strideX, currentState.OrientationOrigin.Y + strideY), rotation);
                } while (c);
                lst.Add(new Order {Rotation = rotation,Stride = stride });
            }
            double finalRotation, finalStride;
            finalRotation = Math.Atan((currentState.OrientationOrigin.X - end.X) / (currentState.OrientationOrigin.Y - end.Y));
            finalStride = Math.Sqrt(Math.Pow((currentState.OrientationOrigin.X - end.X), 2) + Math.Pow((currentState.OrientationOrigin.Y - end.Y), 2));
            lst.Add(new Order() { Rotation = finalRotation, Stride = finalStride });
            return lst;
        }

        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInRadians)
        {
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point(
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y));
            
        }
    }
}
