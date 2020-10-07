using AutoMapper;
using SpeedRegistry.Data;
using System;
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
    }
}
