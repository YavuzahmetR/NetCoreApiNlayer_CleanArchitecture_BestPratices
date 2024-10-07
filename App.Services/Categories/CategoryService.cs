using App.Repositories;
using App.Repositories.Categories;
using App.Repositories.Products;
using App.Services.Categories.Create;
using App.Services.Categories.Dto;
using App.Services.Categories.Update;
using App.Services.Products;
using App.Services.Products.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Categories
{
    public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        public async Task<ServiceResult<int>> CreateAsync(CreateCategoryRequest request)
        {
            var anyCategory = await categoryRepository.Where(x => x.Name == request.Name).AnyAsync();
            if (anyCategory)
            {
                return ServiceResult<int>.Fail($"{request.Name} is Already Exists",
                    HttpStatusCode.BadRequest);
            }

            var newCategory = mapper.Map<Category>(request); 

            await categoryRepository.AddAsync(newCategory);

            await unitOfWork.SaveChangesAsync();

            return ServiceResult<int>.SuccessAsCreated(newCategory.Id,$"api/categories/{newCategory.Id}");
        }
        public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            //var category = await categoryRepository.GetByIdAsync(id);
            //if (category is null)
            //{
            //    return ServiceResult.Fail("Category Not Found", HttpStatusCode.NotFound);
            //}

            var isCategoryNameExists = await categoryRepository.Where(c => c.Name == request.Name && c.Id != id).
                AnyAsync();

            if (isCategoryNameExists)
            {
                return ServiceResult.Fail($"{request.Name} Already Exists", HttpStatusCode.BadRequest);
            }
            var category = mapper.Map<Category>(request);
            category.Id = id;
            categoryRepository.Update(category);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            //if (category is null)
            //{
            //    return ServiceResult.Fail("Category Not Found", HttpStatusCode.NotFound);
            //}

            

            categoryRepository.Delete(category);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult<List<CategoryDto>>> GetAllListAsync()
        {
            var categories = await categoryRepository.GetAll().ToListAsync();

            #region manuelmapping
            //var productAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
            #endregion

            var categoriesAsDto = mapper.Map<List<CategoryDto>>(categories);

            return ServiceResult<List<CategoryDto>>.Success(categoriesAsDto);
        }
        public async Task<ServiceResult<CategoryDto?>> GetByIdAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);

            if (category is null)
            {
                return ServiceResult<CategoryDto?>.Fail("Category Not Found", System.Net.HttpStatusCode.NotFound);
            }
            #region manuelmapping
            //var productsAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
            #endregion

            var categoryAsDto = mapper.Map<CategoryDto>(category);
            return ServiceResult<CategoryDto>.Success(categoryAsDto)!;

        }

        public async Task<ServiceResult<CategoryWithProductDto>> GetCategoryWithProductsAsync(int categoryId)
        {
            var category = await categoryRepository.GetCategoryWithProductsAsync(categoryId);
            if (category is null)
            {
                return ServiceResult<CategoryWithProductDto>.Fail("Category Not Found", HttpStatusCode.NotFound);
            }
            var categoryAsDto = mapper.Map<CategoryWithProductDto>(category);
            return ServiceResult<CategoryWithProductDto>.Success(categoryAsDto);
        }
        public async Task<ServiceResult<List<CategoryWithProductDto>>> GetCategoryWithProducts()
        {
            var category = await categoryRepository.GetCategoryWithProducts().ToListAsync();
            
            var categoryAsDto = mapper.Map<List<CategoryWithProductDto>>(category);

            return ServiceResult<List<CategoryWithProductDto>>.Success(categoryAsDto);
        }

    }
}
