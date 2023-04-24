using Shop.Api.Models.ListLog;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.Page;
using OneOf;

namespace Shop.Api.Abtracst
{
    public interface IUserServices
    {
        public Task<string> SignUpAsync(SignUpUserModel user);
        public Task<string> SignUpADAsync(SignUpUserModel user);
        public Task<AuthenRespone> SignInAsync(SignInUserModel user);
        public Task<string> SignUpUserAsync(SignUpUserModel user);
        public Task<ProfileUserDTO> GetProfileUser(string Email);
        public Task<string> SetProfileUser(SignUpUserModel user, string mail);
        public Task<AuthenRespone> RefreshTokenAysnc(AuthenRespone authenRefresh);
        public Task<ResponseUser> ReVokeAsync(string email);
        public Task<UserApp> GetUserByEmailAsync(string email);
        public Task<PagedList<UserApp>> GetAllUserAsync(int page, int pageSize);
        public Task<bool> ChangeStatusAsync(string email, bool status);
        public Task<bool> ChangePassword(string email, string currentPassword, string newPassword);
        public Task<string> ResetPasswordAsync(string email);
        public Task<string> ChangResetPasswordAsync(ResetPasswordModel request);
    }
}
