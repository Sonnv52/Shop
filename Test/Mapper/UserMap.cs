using AutoMapper;
using Test.Data;
using Test.Models;

namespace Test.Mapper
{
    public class UserMap : Profile
    {
        public UserMap()
        {
            CreateMap<ProfileUser, UserApp>().ReverseMap();
        }
    }
}
