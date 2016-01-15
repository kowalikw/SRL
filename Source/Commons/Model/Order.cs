using System;
using System.Windows;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    public struct Order : IEquatable<Order>
    {
        public double Rotation { get; }
        public Point Destination { get; }

        public Order(double rotation, Point destination)
        {
            Rotation = rotation;
            Destination = destination;
        }

        public bool Equals(Order other)
        {
            return Rotation == other.Rotation &&
                Destination == other.Destination;
        }
    }
}
