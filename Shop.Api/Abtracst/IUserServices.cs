using Shop.Api.Models.ListLog;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.Page;

namespace Shop.Api.Abtracst
{
    public interface IUserServices
    {
        public Task<string> SignUpAsync(SignUpUser user);
        public Task<string> SignUpADAsync(SignUpUser user);
        public Task<AuthenRespone> SignInAsync(SignInUser user);
        public Task<string> SignUpUserAsync(SignUpUser user);
        public Task<ProfileUser> GetProfileUser(string Email);
        public Task<string> SetProfileUser(SignUpUser user, string mail);
        public Task<AuthenRespone> RefreshTokenAysnc(AuthenRespone authenRefresh);
        public Task<ResponseUser> ReVokeAsync(string email);
        public Task<UserApp> GetUserByEmailAsync(string email);
        public Task<PagedList<UserApp>> GetAllUserAsync(int page, int pageSize);
        public Task<bool> ChangeStatusAsync(string email, bool status);
        public Task<bool> ChangePassword(string email, string currentPassword, string newPassword);
    }
}
