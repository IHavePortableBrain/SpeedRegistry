using System;

namespace SpeedRegistry.Business.Dto
{
    public class SpeedEntryDto
    {
        public DateTime DateTime { get; set; }

        public string VehicleNumber { get; set; }

        public double Speed { get; set; }
    }
}
