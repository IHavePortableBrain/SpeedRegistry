using SpeedRegistry.Core;

namespace SpeedRegistry.Business.Dto
{
    public class GetMinMaxSpeedEntryDto
    {
        public ClosedPeriod Period { get; set; }

        public int MaxSpeed { get; set; }
    }
}
