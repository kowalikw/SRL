using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace SRL.Commons.Model.Base
{
    public interface IAlgorithm
    {
        string GetKey { get; }

        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleSize, double vehicleRotation, CancellationToken token);

        List<Option> GetOptions();
        void SetOptions(List<Option> options);
    }
}
