using AutoMapper;
using Shop.Api.Models;
using Shop.Api.Data;

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
