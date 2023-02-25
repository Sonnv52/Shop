using Microsoft.AspNetCore.Identity;
using Test.Models;

namespace Test.Repository
{
    public interface IUserRepository
    {
        public Task<string> SignUpAsync(SignUpUser user);
        public Task<string> SignInAsync(SignInUser user);
        public Task<string> SignUpUserAsync(SignUpUser user);
        public Task<ProfileUser> GetProfileUser(string Email);
    }
}
