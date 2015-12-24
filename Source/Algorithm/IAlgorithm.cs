using System.Collections.Generic;
using SRL.Commons.Model;
using System.Windows;


namespace SRL.Algorithm
{
    interface IAlgorithm
    {
        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity);
    }
}
