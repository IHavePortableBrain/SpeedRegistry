using Microsoft.Extensions.DependencyInjection;
using SpeedRegistry.Data.Repositories;
using System;

namespace SpeedRegistry.Data.FileSystem
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IServiceProvider ServiceProvider { get; }

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IUnitOfWork Build(bool shareConnection = false)
        {
            var speedEntryRepository = ServiceProvider.GetRequiredService<ISpeedEntryRepository>();

            return new UnitOfWork(speedEntryRepository);
        }
    }
}
