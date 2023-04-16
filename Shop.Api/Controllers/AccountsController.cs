using ForgotPasswordService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Caching.Distributed;
using Shop.Api.Abtracst;
using System.Security.Claims;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.ListLog;
using Shop.Api.Repository;
using Shop.Api.Models.Page;
using Shop.Api.Models.Order;

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
        public async Task<IActionResult> SignUpUserAsync(SignUpUser user)
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
        public async Task<IActionResult> SignInAsync(SignInUser user)
        {
            var result = await _userRepository.SignInAsync(user);
            if(result == null || String.IsNullOrEmpty(result.Token))
            {
                return StatusCode(403, "Can't authen this account");
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
    }
}
