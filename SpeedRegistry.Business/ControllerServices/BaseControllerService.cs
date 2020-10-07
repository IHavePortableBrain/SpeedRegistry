using AutoMapper;
using SpeedRegistry.Data;

namespace SpeedRegistry.Business.ControllerServices
{
    public abstract class BaseControllerService
    {
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }
        protected IMapper Mapper { get; }

        public BaseControllerService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            Mapper = mapper;
        }
    }
}
