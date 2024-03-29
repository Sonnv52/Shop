﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shop.Api.Abtracst;
using Shop.Api.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shop.Api.MilderWare
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _configuration = configuration;
            _httpContextAccessor= httpContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            
            if (!context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                await _next(context);
                return;
            }

            var jwt = token.ToString().Replace("Bearer ", "");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? ""))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var _userManager = _httpContextAccessor.HttpContext.RequestServices.GetService<UserManager<UserApp>>();
                var claimsPrincipal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);

                var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                string Name = userIdClaim.Subject.Name.ToString();

                // Check if user exists and is not deleted
                var user = await _userManager.FindByEmailAsync(Name);
                if (user == null || !user.LockoutEnabled)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                // Set user in the context
                context.Items["User"] = user;

                await _next(context);
            }
            catch (SecurityTokenException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder JwtAuthenticationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtAuthenticationMiddleware>();
        }
    }
}
