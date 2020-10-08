using SpeedRegistry.Core;
using SpeedRegistry.Data.Entites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedRegistry.Data.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> FilterAsync(ClosedPeriod period, Func<T, bool> predicate);
    }
}
