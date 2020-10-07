using System;

namespace SpeedRegistry.Data.Entites
{
    public class SpeedEntry
    {
        public DateTime DateTime { get; set; }

        public string VehicleNumber { get; set; } // FK?

        public double Speed { get; set; }
    }
}
