using SpeedRegistry.Core;
using SpeedRegistry.Data.Entites;
using System.Collections.Generic;

namespace SpeedRegistry.Data.Repositories
{
    public interface ISpeedEntryRepository : IRepository<SpeedEntry>, ILastMethodElapsed
    {
        void CreateSortedRangeAsync(IEnumerable<SpeedEntry> entities);
    }
}
