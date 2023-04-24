using AutoMapper;
using Shop.Api.Data;
using Shop.Api.Models;
namespace Shop.Api.Mapper
{
    public class ProductMap: Profile
    {
        public ProductMap() {
            CreateMap<ProductOnlyDTO, Product>().ReverseMap();
            CreateMap<SizeDTO, Size>().ReverseMap();
        }
    }
}
