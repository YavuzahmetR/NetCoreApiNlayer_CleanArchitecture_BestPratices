using App.Services.Categories;
using App.Services.Products.Create;
using App.Services.Products.Update;
using App.Services.Products;
using App.Services.UpdateStock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Services.Categories.Create;
using App.Services.Categories.Update;
using App.Repositories.Products;
using App.Services.Filters;
using App.Repositories.Categories;

namespace App.API.Controllers
{

    public class CategoriesController(ICategoryService categoryService) : CustomBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll() => CreateActionResult(await categoryService.GetAllListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) => CreateActionResult(await categoryService.GetByIdAsync(id));

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetCategoriesWithProducts(int id) => CreateActionResult(await categoryService.GetCategoryWithProductsAsync(id));

        [HttpGet("products")]
        public async Task<IActionResult> GetCategoriesWithProducts() => CreateActionResult(await categoryService.GetCategoryWithProducts());

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request) =>
            CreateActionResult(await categoryService.CreateAsync(request));


        [ServiceFilter(typeof(NotFoundFilter<Category, int>))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryRequest request) =>
            CreateActionResult(await categoryService.UpdateAsync(id, request));


        [ServiceFilter(typeof(NotFoundFilter<Category, int>))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) => CreateActionResult(await categoryService.DeleteAsync(id));
    }
}
