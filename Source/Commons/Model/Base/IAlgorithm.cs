using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace SRL.Commons.Model.Base
{
    /// <summary>
    /// Defines a common interface for classes that calculate path of a vehicle in a labirynth.
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// Implementation key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Calculates and returns path for the setup passed via the parameters and options set with <see cref="SetOptions"/> method.
        /// </summary>
        /// <param name="map">Map with obstacles to avoid.</param>
        /// <param name="vehicle">Vehicle definition.</param>
        /// <param name="start">Starting point.</param>
        /// <param name="end">Destination of the vehicle.</param>
        /// <param name="vehicleSize">Vehicle resize factor.</param>
        /// <param name="vehicleRotation">Initial vehicle orientation.</param>
        /// <param name="token">Token used to cancel ongoing path calculation.</param>
        /// <returns>List of orders for the vehicle.</returns>
        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, double vehicleSize, double vehicleRotation, CancellationToken token);

        /// <summary>
        /// Gets the default options of the specific algorithm implementation.
        /// </summary>
        /// <returns>List of options.</returns>
        List<Option> GetOptions();

        /// <summary>
        /// Sets modified algorithm options previously obtained with <see cref="GetOptions"/> method.
        /// </summary>
        /// <param name="options">Options to set.</param>
        void SetOptions(List<Option> options);
    }
}
