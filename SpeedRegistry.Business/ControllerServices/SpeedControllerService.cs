using AutoMapper;
using SpeedRegistry.Data;

namespace SpeedRegistry.Business.ControllerServices
{
    public class SpeedControllerService: BaseControllerService
    {
        public SpeedControllerService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper) 
            : base(unitOfWorkFactory, mapper) 
        {
        }
    }
}
