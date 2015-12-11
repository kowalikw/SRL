using System.Collections.Generic;
using SRL.Model.Model;

namespace SRL.Model
{

    public interface IAlgorithm
    {
        /// <summary>
        /// Constructs a path that guides the given vehicle from start to end without obstructing any of the map's obstacles.
        /// </summary>
        /// <param name="map">Definition of the map.</param>
        /// <param name="vehicle">Definition of the vehicle.</param>
        /// <param name="start">Path's endpoint from which the vehicle starts moving.</param>
        /// <param name="end">Path's destination endpoint.</param>
        /// <param name="vehicleRotation">Initial rotation of the vehicle (in radians).</param>
        /// <param name="angleDensity">Rotation accuracy - number of angles that divide the 360 degree spectrum. A value of 360 
        /// means that the smallest turn the vehicle can take is 1 degree.</param>
        /// <returns>A list of orders that make up the path for the vehicle.</returns>
        List<Order> GetPath(Map map, Vehicle vehicle, Point start, Point end, Point vehicleRotation, int angleDensity);
    }
}
