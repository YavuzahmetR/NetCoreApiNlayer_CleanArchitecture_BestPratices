using App.Application.Contracts.Persistence;
using App.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Persistence
{
    public class GenericRepository<T,TId>(AppDbContext context) :
        IGenericRepository<T, TId> where T : BaseEntity<TId> where TId : struct
   
    {
        protected AppDbContext Context = context;

        private readonly DbSet<T> _dbSet = context.Set<T>();

        public Task<bool> AnyAsync(TId id) => _dbSet.AnyAsync(x => x.Id.Equals(id));
        public async ValueTask AddAsync(T Entity) => await _dbSet.AddAsync(Entity);


        public void Delete(T Entity) => _dbSet.Remove(Entity);
        

        public IQueryable<T> GetAll() => _dbSet.AsQueryable().AsNoTracking();


        public ValueTask<T?> GetByIdAsync(int id) => _dbSet.FindAsync(id);


        public void Update(T Entity) => _dbSet.Update(Entity);
        

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).AsNoTracking();

        public Task<List<T>> GetAllAsync() => _dbSet.ToListAsync();
        

        public Task<List<T>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            return _dbSet.Skip((pageNumber - 1)* pageSize).Take(pageSize).ToListAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => _dbSet.AnyAsync(predicate);
        
    }
}
