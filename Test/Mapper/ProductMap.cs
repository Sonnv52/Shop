using AutoMapper;
using Test.Data;
using Test.Models;

namespace Test.Mapper
{
    public class ProductMap: Profile
    {
        public ProductMap() {
            CreateMap<ProductT, Product>().ReverseMap();
        }
    }
}
