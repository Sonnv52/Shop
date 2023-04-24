using AutoMapper;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Mapper
{
    public class UserMap : Profile
    {
        public UserMap()
        {
            CreateMap<ProfileUserDTO, UserApp>().ReverseMap();
        }
    }
}
