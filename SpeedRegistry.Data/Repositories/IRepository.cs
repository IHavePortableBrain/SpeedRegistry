using SpeedRegistry.Data.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedRegistry.Data.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        Task<T> CreateAsync(T entity);

        void CreateRangeAsync(IEnumerable<T> entities);

        Task<IEnumerable<SpeedEntry>> FilterAsync(Core.Period period);
    }
}
