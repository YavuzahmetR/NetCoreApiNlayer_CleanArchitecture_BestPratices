using System.Linq.Expressions;

namespace App.Application.Contracts.Persistence
{
    public interface IGenericRepository<T,TId> where T : class where TId : struct
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetPagedAllListAsync(int pageNumber, int pageSize);
        public Task<bool> AnyAsync(TId id);
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        ValueTask<T?> GetByIdAsync(int id);
        ValueTask AddAsync(T Entity);
        void Update(T Entity);
        void Delete(T Entity);
    }
}
