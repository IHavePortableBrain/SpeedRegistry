using SpeedRegistry.Data.Entites;
using SpeedRegistry.Data.Repositories;

namespace SpeedRegistry.Data.FileSystem
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<SpeedEntry> SpeedEntryRepository { get; }

        public UnitOfWork(IRepository<SpeedEntry> speedEntryRepository)
        {
            SpeedEntryRepository = speedEntryRepository;
        }

        public void Dispose()
        {
        }
    }
}
