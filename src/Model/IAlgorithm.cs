using System.Collections.Generic;
using SRL.Model.Model;

namespace SRL.Model
{
    public interface IAlgorithm
    {
        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, Point vehicleRotation);
    }
}
