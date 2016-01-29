namespace SRL.Commons.Model
{
    /// <summary>
    /// Helper model class used in setting initial vehicle rotation and vehicle size at once.
    /// </summary>
    public class VehicleSetup
    {
        /// <summary>
        /// Initial orientation of the vehicle.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Vehicle resize factor.
        /// </summary>
        public double RelativeSize { get; set; }
    }
}
