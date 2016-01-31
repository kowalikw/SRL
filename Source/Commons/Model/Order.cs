using System;
using System.Windows;

namespace SRL.Commons.Model
{
    /// <summary>
    /// Model class that represents a pair of actions that move the vehicle on a map.
    /// </summary>
    public struct Order : IEquatable<Order>
    {
        /// <summary>
        /// Absolute angle (in radians) to which the vehicle must rotate before making move to <see cref="Destination"/>. The sign of the value indicates whether the turn is counter-clockwise (no sign) or clockwise (negative).
        /// </summary>
        /// <remarks>Values must fall in range from -360 (inclusive) to 360 (exclusive).</remarks>
        public double Rotation { get; }
        /// <summary>
        /// A <see cref="Point"/> to which the vehicle should move after making the turn specified by <see cref="Rotation"/> value.
        /// </summary>
        public Point Destination { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> structure.
        /// </summary>
        /// <param name="rotation">Angle from which the move shall be made.</param>
        /// <param name="destination">Move destination.</param>
        public Order(double rotation, Point destination)
        {
            Rotation = rotation;
            Destination = destination;
        }

        /// <inheritdoc />
        public bool Equals(Order other)
        {
            return Rotation == other.Rotation &&
                Destination == other.Destination;
        }
    }
}
