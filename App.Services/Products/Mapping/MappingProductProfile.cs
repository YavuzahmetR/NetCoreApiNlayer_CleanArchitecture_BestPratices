using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Products.Mapping
{
    public class MappingProductProfile : Profile
    {
        public MappingProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<CreateProductRequest, Product>().ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));

            CreateMap<UpdateProductRequest, Product>().ForMember(dest => dest.Name,
               opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));
        }
    }
}
