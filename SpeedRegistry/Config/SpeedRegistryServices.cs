using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpeedRegistry.Business.ControllerServices;
using SpeedRegistry.Data;
using SpeedRegistry.Data.FileSystem;
using SpeedRegistry.Data.FileSystem.Repositories;
using SpeedRegistry.Data.Repositories;
using System;

namespace SpeedRegistry.Config
{
    public static class SpeedRegistryServices
    {
        public static void AddSpeedRegistryServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Repositories
            services.AddScoped<ISpeedEntryRepository, SpeedEntryRepository>();

            // ControllerServices
            services.AddScoped<SpeedControllerService>();

            // Services
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            // Misc
            services.AddSingleton<Random>();
        }
    }
}
