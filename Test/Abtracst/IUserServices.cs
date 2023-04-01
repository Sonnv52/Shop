using Shop.Api.Models.ListLog;
using Test.Data;
using Test.Models;

namespace Shop.Api.Abtracst
{
    public interface IUserServices
    {
        public Task<string> SignUpAsync(SignUpUser user);
        public Task<AuthenRespone> SignInAsync(SignInUser user);
        public Task<string> SignUpUserAsync(SignUpUser user);
        public Task<ProfileUser> GetProfileUser(string Email);
        public Task<string> SetProfileUser(SignUpUser user, string mail);
        public Task<AuthenRespone> RefreshTokenAysnc(AuthenRespone authenRefresh);
        public Task<ResponseUser> ReVokeAsync(string email);
        public Task<UserApp> GetUserByEmailAsync(string email);
    }
}
