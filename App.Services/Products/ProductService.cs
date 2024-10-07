using App.Repositories;
using App.Repositories.Products;
using App.Services.ExceptionHandlers;
using App.Services.Products.Create;
using App.Services.Products.Update;
using App.Services.UpdateStock;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace App.Services.Products
{
    public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count)
        {
            var products = await productRepository.GetTopPriceProductAsync(count);

            #region manuelmapping
            //var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);    

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }
        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            var products = await productRepository.GetAll().ToListAsync();

            #region manuelmapping
            //var productAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            var products = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            #region manuelmapping
            //var productAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }
        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return ServiceResult<ProductDto?>.Fail("Product Not Found", System.Net.HttpStatusCode.NotFound);
            }
            #region manuelmapping
            //var productsAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
            #endregion

            var productsAsDto = mapper.Map<ProductDto>(product);
            return ServiceResult<ProductDto>.Success(productsAsDto)!;

        }
        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            //throw new CriticalException("Kritik Seviye Hata");// 

            //business async validation check
            var anyProduct = await productRepository.Where(x => x.Name == request.Name).AnyAsync();
            if (anyProduct)
            {
                return ServiceResult<CreateProductResponse>.Fail($"{request.Name} is Already Exists",
                    HttpStatusCode.BadRequest);
            }


            var product = mapper.Map<Product>(request);

            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),
                $"api/products/{product.Id}");
        }
        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            //var product = await productRepository.GetByIdAsync(id);

            //if (product is null)
            //{
            //    return ServiceResult.Fail("Product Not Found", HttpStatusCode.NotFound);
            //}

            var isProductNameExists = await productRepository.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();
            if (isProductNameExists)
            {
                return ServiceResult.Fail($"{request.Name} is Already Exists",
                    HttpStatusCode.BadRequest);
            }


            //product.Name = request.Name;
            //product.Price = request.Price;
            //product.Stock = request.Stock;

            var product = mapper.Map<Product>(request);
            product.Id = id;
            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);
            if (product is null)
            {
                return ServiceResult.Fail($"Product {request.ProductId} does not exist", HttpStatusCode.NotFound);
            }
            product.Stock = request.Quantity;
            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            //if (product is null) -------- FILTER EXISTS
            //{
            //    return ServiceResult.Fail("Product Not Found", HttpStatusCode.NotFound);
            //}

            productRepository.Delete(product!);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
