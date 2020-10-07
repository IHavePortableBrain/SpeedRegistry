using SpeedRegistry.Data.Entites;
using SpeedRegistry.Data.Repositories;

namespace SpeedRegistry.Data.FileSystem.Repositories
{
    public class SpeedEntryRepository : Repository<SpeedEntry>, ISpeedEntryRepository
    {
        public SpeedEntryRepository()
        {
        }
    }
}
