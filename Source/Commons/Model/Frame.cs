using System.Windows;

namespace SRL.Commons.Model
{
    /// <summary>
    /// Mocel class that represents <see cref="Vehicle"/> orientation during a single simulation timeframe.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Non-negative absolute angle (in radians) to which the vehicle is rotated.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Current position of the vehicle.
        /// </summary>
        public Point Position { get; set; }
    }
}
