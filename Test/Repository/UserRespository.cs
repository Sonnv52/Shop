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
using Shop.Api.Abtracst;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Models.ListLog;

namespace Test.Repository
{
    public class UserRespository : IUserServices
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAccount _account;
        public UserRespository(UserManager<UserApp> userManager, SignInManager<UserApp> signInManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper, IAccount account, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _mapper = mapper;
            _account = account;

        }
        public async Task<string> SignUpAsync(SignUpUser model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return "Exsit!!!";

            UserApp user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Name = model.Name,
                Adress = model.Adress,
                PhoneNumber= model.PhoneNumber
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

        public async Task<AuthenRespone> SignInAsync(SignInUser model)
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
                var Refreshtoken = await Task.Run(() => GenerateRefreshToken());
                user.RefreshToken = Refreshtoken;
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                await _userManager.UpdateAsync(user);
                var ResponeToken = new AuthenRespone
                {
                    User = model.Email,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = Refreshtoken
                };
                return ResponeToken;
            }
            return new AuthenRespone
            {
                Token = "false"
            };
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
#pragma warning restore CS8604 // Possible null reference argument.

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
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
            if (user.Adress != null)
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
            if (!result.Succeeded) return "false";
            return "success";
        }

        public async Task<AuthenRespone> RefreshTokenAysnc(AuthenRespone authenRefresh)
        {
            string? accessToken = authenRefresh.Token;
            string? refreshToken = authenRefresh.RefreshToken;
            var principal = await Task.Run(() => GetPrincipalFromExpiredToken(accessToken));
            if (principal == null)
            {
                return new AuthenRespone
                {
                    Token = "Invalid"
                };
            }
#pragma warning disable CS8600
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string username = principal.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new AuthenRespone
                {
                    Token = "Invalid access token or refresh tokenInvalid access token or refresh token"
                };
            }

            var newAccessToken = GetToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthenRespone
            {
                User = authenRefresh.User,
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };

        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };
#pragma warning restore CS8604 // Possible null reference argument.

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

        public async Task<ResponseUser> ReVokeAsync(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null) return new ResponseUser
            {
                Status = "false",
                Message = new List<string>()
            };

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return new ResponseUser
            {
                Status = "true"
            };
        }

        public async Task<UserApp> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            return user;
        }
    }
}
