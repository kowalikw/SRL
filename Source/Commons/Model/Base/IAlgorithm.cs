using System.Collections.Generic;
using System.Windows;

namespace SRL.Commons.Model.Base
{
    public interface IAlgorithm
    {
        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleSize, double vehicleRotation);

        List<AlgorithmOption> GetOptions();
        void SetOptions(List<AlgorithmOption> options);
    }
}
