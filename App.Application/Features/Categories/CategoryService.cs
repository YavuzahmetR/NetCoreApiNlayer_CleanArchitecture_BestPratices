using App.Application.Contracts.Persistence;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Dto;
using App.Application.Features.Categories.Update;
using App.Domain;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Categories
{
    public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        public async Task<ServiceResult<int>> CreateAsync(CreateCategoryRequest request)
        {
            var anyCategory = await categoryRepository.AnyAsync(x => x.Name == request.Name);
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

            var isCategoryNameExists = await categoryRepository.AnyAsync(c => c.Name == request.Name && c.Id != id);
                

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
            
            categoryRepository.Delete(category!);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult<List<CategoryDto>>> GetAllListAsync()
        {
            var categories = await categoryRepository.GetAllAsync();

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
            var category = await categoryRepository.GetCategoryWithProducts();
            
            var categoryAsDto = mapper.Map<List<CategoryWithProductDto>>(category);

            return ServiceResult<List<CategoryWithProductDto>>.Success(categoryAsDto);
        }

    }
}
