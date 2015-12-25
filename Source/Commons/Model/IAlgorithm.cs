using System.Collections.Generic;
using System.Windows;

namespace SRL.Commons.Model
{
    public interface IAlgorithm
    {
        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity);
    }
}
