using AutoMapper;
using SpeedRegistry.Business.Dto;
using SpeedRegistry.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedRegistry.Business.ControllerServices
{
    public class SpeedControllerService: BaseControllerService
    {
        public SpeedControllerService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper) 
            : base(unitOfWorkFactory, mapper) 
        {

        }

        public async Task SaveSomeSpeedEntriesAsync()
        {
            using (var uow = UnitOfWorkFactory.Build())
            {
                await uow.SpeedEntryRepository.CreateAsync(new Data.Entites.SpeedEntry() { DateTime = DateTime.UtcNow });
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
    }
}
