using AutoMapper;
using SpeedRegistry.Business.Dto;
using SpeedRegistry.Core;
using SpeedRegistry.Data;
using SpeedRegistry.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedRegistry.Business.ControllerServices
{
    public class SpeedControllerService: BaseControllerService
    {
        private readonly Random _random;

        public SpeedControllerService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IMapper mapper,
            Random random)
            : base(unitOfWorkFactory, mapper) 
        {
            _random = random;
        }

        public async Task CreateSpeedEntryAsync(SpeedEntryDto dto)
            => await CreateSpeedEntriesAsync(new List<SpeedEntryDto> { dto });

        public async Task CreateSpeedEntriesAsync(IEnumerable<SpeedEntryDto> dtos)
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var entities = Mapper.Map<IEnumerable<SpeedEntry>>(dtos);
                await uow.SpeedEntryRepository.CreateRangeAsync(entities);
            } 
        }

        public async Task<IEnumerable<SpeedEntryDto>> GetAllEntriesAsync()
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var now = DateTime.UtcNow;
                var entries = await uow.SpeedEntryRepository
                    .FilterAsync(
                        new ClosedPeriod() { From = DateTime.UtcNow.AddDays(-100), To = DateTime.UtcNow.AddDays(100) },
                        se => true);
                return  Mapper.Map<IEnumerable<SpeedEntryDto>>(entries);
            }
        }

        public async Task<MinMaxSpeedEntryDto> GetMinMaxSpeedEntriesAsync(ClosedPeriod period)
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var now = DateTime.UtcNow;
                var entries = await uow.SpeedEntryRepository
                    .FilterAsync(
                        period,
                        se => true);
                var result = new MinMaxSpeedEntryDto()
                {
                    Min = Mapper.Map<SpeedEntryDto>(entries.Aggregate((min, e) => e.Speed < min.Speed ? e : min)),
                    Max = Mapper.Map<SpeedEntryDto>(entries.Aggregate((max, e) => e.Speed > max.Speed ? e : max))
                };

                return result;
            }
        }

        public async Task<IEnumerable<SpeedEntryDto>> GetOverSpeedEntriesAsync(GetMinMaxSpeedEntryDto dto)
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var now = DateTime.UtcNow;
                var entries = await uow.SpeedEntryRepository
                    .FilterAsync(
                        dto.Period,
                        se => se.Speed >= dto.MaxSpeed);

                return Mapper.Map<IEnumerable<SpeedEntryDto>>(entries);
            }
        }

        public async Task CreateTestEntriesAsync()
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var fromTicks = DateTime.UtcNow.AddDays(-1).Ticks;
                var toTicks = DateTime.UtcNow.AddDays(0).Ticks;
                var entries = new List<SpeedEntry>();
                for (int i = 0; i < 100; i++)
                {
                    entries.Add(new SpeedEntry
                    {
                        DateTime = new DateTime(_random.NextLong(fromTicks, toTicks)),
                        Speed = _random.Next(0, 220),
                        VehicleNumber = _random.NextVehicleNumber(),
                    });
                }

                await uow.SpeedEntryRepository.CreateRangeAsync(entries);
            }
        }
    }
}
