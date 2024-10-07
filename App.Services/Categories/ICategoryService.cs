using App.Services.Categories.Create;
using App.Services.Categories.Dto;
using App.Services.Categories.Update;
using App.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Categories
{
    public interface ICategoryService
    {
        Task<ServiceResult<int>> CreateAsync(CreateCategoryRequest request);
        Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request);
        Task<ServiceResult<List<CategoryDto>>> GetAllListAsync();
        Task<ServiceResult<CategoryDto?>> GetByIdAsync(int id);
        Task<ServiceResult<CategoryWithProductDto>> GetCategoryWithProductsAsync(int categoryId);
        Task<ServiceResult<List<CategoryWithProductDto>>> GetCategoryWithProducts();
        Task<ServiceResult> DeleteAsync(int id);
    }
}
