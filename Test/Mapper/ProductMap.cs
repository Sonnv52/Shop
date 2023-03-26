using AutoMapper;
using Shop.Api.Data;
using Shop.Api.Models;
using Test.Data;
using Test.Models;

namespace Test.Mapper
{
    public class ProductMap: Profile
    {
        public ProductMap() {
            CreateMap<ProductT, Product>().ReverseMap();
            CreateMap<SizeDTO, Size>().ReverseMap();
        }
    }
}
