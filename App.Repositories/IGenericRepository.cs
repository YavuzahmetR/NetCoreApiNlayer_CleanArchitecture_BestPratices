using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories
{
    public interface IGenericRepository<T,TId> where T : class where TId : struct
    {
        IQueryable<T> GetAll();

        public Task<bool> AnyAsync(TId id);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        ValueTask<T?> GetByIdAsync(int id);
        ValueTask AddAsync(T Entity);
        void Update(T Entity);
        void Delete(T Entity);
    }
}
