using App.Application.Contracts.Caching;
using App.Application.Contracts.Persistence;
using App.Application.Contracts.ServiceBus;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Dto;
using App.Application.Features.Products.Update;
using App.Domain;
using App.Domain.Events;
using AutoMapper;
using System;
using System.Net;


namespace App.Application.Features.Products
{
    public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper,
        ICacheService cacheService, IServiceBus serviceBus) : IProductService
    {
        private const string ProductListCacheKey = "ProductListCacheKey";
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

            //Decorator pattern - Proxy pattern More Advanced and Highly recommended!
            //cache aside design pattern - 1 any cache, 2 from db, 3 cache data

            var productListAsCached = await cacheService.GetAsync<List<ProductDto>>(ProductListCacheKey);

            if (productListAsCached is not null) return ServiceResult<List<ProductDto>>.Success(productListAsCached);

            //cache aside design pattern

            var products = await productRepository.GetAllAsync();

            #region manuelmapping
            //var productAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
            #endregion

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            await cacheService.AddAsync(ProductListCacheKey, productsAsDto, TimeSpan.FromMinutes(1));

            return ServiceResult<List<ProductDto>>.Success(productsAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            var products = await productRepository.GetPagedAllListAsync(pageNumber, pageSize);

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
            var anyProduct = await productRepository.AnyAsync(x => x.Name == request.Name);
            if (anyProduct)
            {
                return ServiceResult<CreateProductResponse>.Fail($"{request.Name} is Already Exists",
                    HttpStatusCode.BadRequest);
            }


            var product = mapper.Map<Product>(request);

            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            await serviceBus.PublishAsync(new ProductAddedEvent(product.Id, product.Name, product.Price));

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),
                $"api/products/{product.Id}");
        }
        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {

            var isProductNameExists = await productRepository.AnyAsync(x => x.Name == request.Name && x.Id != id);
            if (isProductNameExists)
            {
                return ServiceResult.Fail($"{request.Name} is Already Exists",
                    HttpStatusCode.BadRequest);
            }

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
