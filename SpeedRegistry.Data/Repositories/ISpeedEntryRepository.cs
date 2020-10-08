using SpeedRegistry.Core;
using SpeedRegistry.Data.Entites;

namespace SpeedRegistry.Data.Repositories
{
    public interface ISpeedEntryRepository : IRepository<SpeedEntry>, ILastMethodElapsed
    {
    }
}
