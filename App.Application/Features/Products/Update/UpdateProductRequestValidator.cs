using FluentValidation;


namespace App.Application.Features.Products.Update
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("Product's Name Must Be Declared").
               NotEmpty().WithMessage("Product's Name Must Be Declared").Length(3, 10).
               WithMessage("Product's Name Must Contain At Least 3 At Most 10 Letters");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Product's Price Must Be Given").GreaterThan(0).
                WithMessage("Product's Price Must Be Greater Than 0");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Product's Category Id Must Be Given").GreaterThan(0).
                WithMessage("Product's Category Id Must Be Greater Than 0");
            RuleFor(x => x.Stock).NotEmpty().WithMessage("Product's Stock Must Be Given").InclusiveBetween(1, 100).
                WithMessage("Product's Stock Number Must Be Between 1 and 100");
        }
    }
}
