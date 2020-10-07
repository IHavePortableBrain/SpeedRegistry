using SpeedRegistry.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpeedRegistry.Data.FileSystem.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        public Repository()
        {
        }
    }
}
