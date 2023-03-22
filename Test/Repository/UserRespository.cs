using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System.Web.Http.ModelBinding;
using Test.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test.Data;
using System.Data;
using Azure;
using AutoMapper;
using ForgotPasswordService.Repository;
using Shop.Api.Models.CreateModel;

namespace Test.Repository
{
    public class UserRespository : IUserRepository
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAccount _account;
        public UserRespository(UserManager<UserApp> userManager,SignInManager<UserApp> signInManager,
            RoleManager<IdentityRole> roleManager,IMapper mapper,IAccount account, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _mapper = mapper;
            _account= account;

        }
        public async Task<string> SignUpAsync(SignUpUser model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return "Exsit";

            UserApp user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Name= model.Name,
                Adress = model.Adress
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return "false";
      
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return "true";
    }

        public async Task<string> SignInAsync(SignInUser model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = GetToken(authClaims);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return "false";
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        public async Task<string> SignUpUserAsync(SignUpUser model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return "Exsit";
            UserApp user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Name = model.Name,
                Adress = model.Adress
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return "false";
            return "success";
        }

        public async Task<ProfileUser> GetProfileUser(string Email)
        {
            var userExists = await _userManager.FindByNameAsync(Email);
            var user = _mapper.Map<ProfileUser>(userExists);
            return user;
        }

        public async Task<string> SetProfileUser(SignUpUser user, string mail)
        {
            var usercurent = await _userManager.FindByEmailAsync(mail);
            if(user.Adress != null)
            {
                usercurent.Adress = user.Adress;
            }
            if (user.PhoneNumber != null)
            {
                usercurent.PhoneNumber = user.PhoneNumber;
            }
            if (user.Name != null)
            {
                usercurent.Name = user.Name;
            }
            var result = await _userManager.UpdateAsync(usercurent);
            if(!result.Succeeded) return "false";
            return "success";
        }


    }
}
