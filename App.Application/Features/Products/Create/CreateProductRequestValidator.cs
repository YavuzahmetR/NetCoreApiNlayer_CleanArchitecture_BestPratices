﻿
using FluentValidation;


namespace App.Application.Features.Products.Create
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        //private readonly IProductRepository _productRepository;
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("Product's Name Must Be Declared").
                NotEmpty().WithMessage("Product's Name Must Be Declared").Length(3, 10).
                WithMessage("Product's Name Must Contain At Least 3 At Most 10 Letters");
            // Must(MustUniqueProductName).WithMessage("This Product Name is Already Exists"); //sync bussiness validation check
            RuleFor(x => x.Price).NotEmpty().WithMessage("Product's Price Must Be Given").GreaterThan(0).
                WithMessage("Product's Price Must Be Greater Than 0");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Product's Category Id Must Be Given").GreaterThan(0).
                WithMessage("Product's Category Id Must Be Greater Than 0");
            RuleFor(x => x.Stock).NotEmpty().WithMessage("Product's Stock Must Be Given").InclusiveBetween(1, 100).
                WithMessage("Product's Stock Number Must Be Between 1 and 100");

        }

        //private bool MustUniqueProductName(string Name)  //sync bussiness validation check
        //{
        //    return !_productRepository.Where(x => x.Name == Name).Any();
        //    //false => returns fail
        //    //true => returns success
        //}
    }
}
