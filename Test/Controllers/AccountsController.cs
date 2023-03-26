using ForgotPasswordService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Caching.Distributed;
using Shop.Api.Abtracst;
using System.Security.Claims;
using Test.Data;
using Test.Models;
using Test.Models.ListLog;
using Test.Repository;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {   
        private readonly IUserServices _userRepository;
        private readonly NewDBContext _newDBContext;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserApp> _userManager;
        private readonly IAccount _account;
        private readonly ILogger<AccountsController> _logger;
        public AccountsController(IUserServices userRepository, NewDBContext newDBContext, IHttpContextAccessor httpContextAccessor,
            UserManager<UserApp> userManager, IAccount account, ILogger<AccountsController> logger)
        {
            _userRepository = userRepository;
            _newDBContext = newDBContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _account = account;
            _logger = logger;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpUser user)
        {
            var result = await _userRepository.SignUpAsync(user);
            if (result == "true")
            {
                return Ok("Suggest!!");
            }
            if(result == "Exsit")
            {
                return BadRequest("Account already exists");
            }
            return Unauthorized();
        }

        [HttpPost("SignUpUser")]
        public async Task<IActionResult> SignUpUserAsync(SignUpUser user)
        {
            string result = await _userRepository.SignUpUserAsync(user);
            if (result == "success")
            {
                return Ok("success!!");
            }
            if(result == "Exsit")
            {
                return BadRequest("Account already exists");
            }    
            return Unauthorized();
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync(SignInUser user)
        {
            var result = await _userRepository.SignInAsync(user);
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            var refreshtoken = "";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            if (result.Token == "false")
            {
                return StatusCode(400, "Password or Email incorect"); ;
            }
           
            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Bearer"),Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetProfileAsync([FromBody] String Email)
        {
            var user = await _userRepository.GetProfileUser(Email);
            if (user == null)
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
            if (result == null)
            {
                return StatusCode(500, "Internal Server Error");
            }
            return Ok(result);
        }

        [HttpPost("ChangeProfile")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> ChangeProfileAsync([FromBody] SignUpUser user)
        {
           string userName = User.FindFirstValue(ClaimTypes.Name);
           string result = await _userRepository.SetProfileUser(user, userName);
            if(result == null)
            {
                return StatusCode(400, "False to save!!");
            }
            if(result == "false")
            {
                return StatusCode(401, "False to save!!");
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
            if(result.RefreshToken == null) {
                return StatusCode(409, result.Token);
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
    }
}
