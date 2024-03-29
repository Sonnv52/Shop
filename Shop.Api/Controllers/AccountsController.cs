﻿using ForgotPasswordService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Abtracst;
using System.Security.Claims;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.Page;
using Shop.Api.Models.Order;
using ForgotPasswordService.Message;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {   
        private readonly IUserServices _userRepository;
        private readonly NewDBContext _newDBContext;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserApp> _userManager;
        private readonly ILogger<AccountsController> _logger;
        private readonly ISendMailService<TokenResetMessage> _send;
        public AccountsController(IUserServices userRepository, NewDBContext newDBContext, IHttpContextAccessor httpContextAccessor,
            UserManager<UserApp> userManager, ILogger<AccountsController> logger, ISendMailService<TokenResetMessage> send)
        {
            _userRepository = userRepository;
            _newDBContext = newDBContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
            _send = send;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpUserModel user)
        {
            var result = await _userRepository.SignUpAsync(user);
            if (result.Equals("true"))
            {
                return Ok("Suggest!!");
            }
            if(result.Equals("Exsit!!!"))
            {
                return BadRequest("Account already exists");
            }
            return Unauthorized();
        }

        [HttpPost("SignUpUser")]
        public async Task<IActionResult> SignUpUserAsync(SignUpUserModel user)
        {
            string result = await _userRepository.SignUpUserAsync(user);
            if (result.Equals("success"))
            {
                return Ok("success!!");
            }
            if(result.Equals("Exsit"))
            {
                return BadRequest("Account already exists");
            }    
            return Unauthorized();
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync(SignInUserModel user)
        {
            var result = await _userRepository.SignInAsync(user);
            if(result is null || String.IsNullOrEmpty(result.Token))
            {
                return StatusCode(403, "Can't authen this account");
            }
            if (result.Token.Equals("false"))
            {
                return StatusCode(500, "Password or Email incorect"); ;
            }
           
            return Ok(result);
        }

        [HttpPost("SignIn/Admin")]
        public async Task<IActionResult> SignInAdminAsync(SignInUserModel user)
        {
            var result = await _userRepository.SignInAsync(user);
            if (result is null || String.IsNullOrEmpty(result.Token))
            {
                return StatusCode(403, "Can't login this account");
            }
            if (result.Token.Equals("false"))
            {
                return StatusCode(500, "Password or Email incorect"); ;
            }

            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Bearer"),Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetProfileAsync([FromBody] String Email)
        {
            var user = await _userRepository.GetProfileUser(Email);
            if (user is null)
            {
                return StatusCode(500, "Internal Server Error");
            }
            return Ok(user);
        }

        [HttpGet("profile/user")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetCurrentProfileAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _userRepository.GetProfileUser(userName);
            if (result is null)
            {
                return StatusCode(500, "Internal Server Error");
            }
            return Ok(result);
        }

        [HttpPost("ChangeProfile")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> ChangeProfileAsync([FromBody] SignUpUserModel user)
        {
           string userName = User.FindFirstValue(ClaimTypes.Name);
           string result = await _userRepository.SetProfileUser(user, userName);
            if(String.IsNullOrEmpty(result))
            {
                return StatusCode(500, "False to save!!");
            }
            if(result.Equals("false"))
            {
                return StatusCode(500, "False to save!!");
            }
            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(AuthenRespone authRefresh)
        {
            if(authRefresh is null)
            {
                return BadRequest();
            }
            var result = await _userRepository.RefreshTokenAysnc(authRefresh);
            if(String.IsNullOrEmpty(result.RefreshToken)) {
                return StatusCode(500, result.Token);
            }
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("Revoke")]
        public async Task<IActionResult> ReVokeAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _userRepository.ReVokeAsync(userName);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("/ListUser")]
        public async Task<IActionResult> GetAllUserAsync([FromQuery]PageQuery page)
        {
            var result = await _userRepository.GetAllUserAsync(page.pageIndex, page.pageSize);
            var pageReturn = new AllUserDTO
            {
                users = result,
                totals = result.TotalPages
            };
            return Ok(pageReturn);
        }

        [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("/ChangeAccount")]
        public async Task<IActionResult> ChangeStatusAccountAsync(string email, bool status)
        {
            var result = await _userRepository.ChangeStatusAsync(email,status);
            return Ok(result);
        }

        [HttpPatch]
        [Route("/ChangePassword")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            string userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _userRepository.ChangePassword(userName,currentPassword, newPassword);
            if(result) 
                return Ok("Done");
            return StatusCode(450, false);
        }

        [HttpPost]
        [Route("/FogotPassWord")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync(string email)
        {
            if(String.IsNullOrEmpty(email)) return BadRequest();
            var tokenReset = await _userRepository.ResetPasswordAsync(email);
            if (tokenReset == null)
                return NotFound("User not found");
            return Ok("Check Your Email!!");
        }

        [HttpPost]
        [Route("/ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordModel model)
        {
            var result = await _userRepository.ChangResetPasswordAsync(model);
            if (result.Equals("true")) return Ok();
            return BadRequest();
        }

    }
}
