using System.Collections.Generic;
using SRL.Model;
using SRL.Model.Model;

namespace SRL.Algorithm
{
    public class MockAlgorithm : IAlgorithm
    {
        public List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleRotation, int angleDensity)
        {
            throw new System.NotImplementedException();
        }
    }
}
