using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Categories
{
    public class CategoryRepository(AppDbContext context) : GenericRepository<Category,int>(context), ICategoryRepository
    {
        public Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return Context.Categories.Include(c => c.Products).FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<Category> GetCategoryWithProducts()
        {
            return Context.Categories.Include(c => c.Products).AsQueryable();
        }
    }
}
