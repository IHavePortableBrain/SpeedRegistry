using AutoMapper;
using SpeedRegistry.Business.Dto;
using SpeedRegistry.Data;
using SpeedRegistry.Data.Entites;
using System;
using System.Collections.Generic;
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

        public async Task CreateSpeedEntryAsync(SpeedEntryDto speedEntryDto)
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var entity = Mapper.Map<SpeedEntry>(speedEntryDto);
                await uow.SpeedEntryRepository.CreateAsync(entity);
            } 
        }

        public async Task<IEnumerable<SpeedEntryDto>> GetSpeedEntriesAsync()
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                var now = DateTime.UtcNow;
                var entries = await uow.SpeedEntryRepository
                    .FilterAsync(
                        new Core.ClosedPeriod() { From = now.AddDays(-1), To = now.AddDays(1) },
                        se => true);
                return  Mapper.Map<IEnumerable<SpeedEntryDto>>(entries);
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
