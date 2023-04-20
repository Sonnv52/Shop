using AutoMapper;
using Shop.Api.Data;
using Shop.Api.Models;
namespace Shop.Api.Mapper
{
    public class ProductMap: Profile
    {
        public ProductMap() {
            CreateMap<ProductT, Product>().ReverseMap();
            CreateMap<SizeDTO, Size>().ReverseMap();
        }
    }
}
