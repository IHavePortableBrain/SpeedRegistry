using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SpeedRegistry.Business.Dto;
using SpeedRegistry.Data.Entites;

namespace SpeedRegistry.Config
{
    public static class Mapper
    {
        public static void AddMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddDalToDtoMappings();
                cfg.AddDtoToDalMappings();
                cfg.AddOtherMappings();
            });

            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);
        }

        public static IProfileExpression AddDalToDtoMappings(this IProfileExpression cfg)
        {
            cfg.CreateMap<SpeedEntry, SpeedEntryDto>();
            return cfg;
        }

        public static IProfileExpression AddDtoToDalMappings(this IProfileExpression cfg)
        {
            cfg.CreateMap<SpeedEntryDto, SpeedEntry>();
            return cfg;
        }

        public static IProfileExpression AddOtherMappings(this IProfileExpression cfg)
        {
            return cfg;
        }
    }
}
