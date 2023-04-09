using AutoMapper;
using Shop.Api.Models;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Mapper
{
    public class BillMap : Profile
    {
        public BillMap()
        {
            CreateMap<BillDTO, Bill>().ReverseMap();
        }
    }
}
