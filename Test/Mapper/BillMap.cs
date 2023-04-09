using AutoMapper;
using Shop.Api.Models;
using Test.Data;
using Test.Models;

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
