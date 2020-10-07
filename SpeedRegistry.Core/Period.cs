using System;

namespace SpeedRegistry.Core
{
    public struct Period
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool Lasts(DateTime? utcNow = null)
        {
            var now = utcNow ?? DateTime.UtcNow;
            return To > From
                ?
                (now.TimeOfDay > From.Value.TimeOfDay) &&
                (now.TimeOfDay < To.Value.TimeOfDay)
                :
                (From.HasValue && now.TimeOfDay > From.Value.TimeOfDay) ||
                (To.HasValue && now.TimeOfDay < To.Value.TimeOfDay) ||
                (!From.HasValue && !To.HasValue);
        }
    }
}
