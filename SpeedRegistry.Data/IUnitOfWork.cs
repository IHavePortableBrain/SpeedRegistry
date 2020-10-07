using SpeedRegistry.Data.Entites;
using SpeedRegistry.Data.Repositories;
using System;

namespace SpeedRegistry.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<SpeedEntry> SpeedEntryRepository { get; }
    }
}
