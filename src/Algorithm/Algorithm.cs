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
        List<Order> IAlgorithm.GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity)
        {
            throw new NotImplementedException();
        }

        void MinkowskiSum(Map map, Vehicle vehicle, double angle)
        {
            List<Point> angledVehicle = new List<Point>();
            for (int i = 0; i < vehicle.Shape.VertexCount; i++)
                angledVehicle.Add(GeometryHelper.RotatePoint(vehicle.Shape.Vertices[i], new Point(0, 0), angle));
            vehicle = new Vehicle(new Polygon(angledVehicle), new Point(0, 0), 0);
            List<List<Polygon>> polygons = new List<List<Polygon>>();
            for (int i = 0; i < map.ObstacleCount; i++)
            {
                for (int j=0;j<map.Obstacles[i].VertexCount;j++)
                {
                    List<Point> shp = new List<Point>();
                    for (int k = 0; k<vehicle.Shape.VertexCount;k++)
                    {
                        shp.Add(vehicle.Shape.Vertices[k] + map.Obstacles[i].Vertices[j]);
                    }
                    polygons[i].Add(new Polygon(shp));
                }
            }


        }

    }
}
