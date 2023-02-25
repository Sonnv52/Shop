using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
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
        private readonly IUserRepository _userRepository;
        private readonly NewDBContext _newDBContext;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserApp> _userManager;
        public AccountsController(IUserRepository userRepository, NewDBContext newDBContext, IHttpContextAccessor httpContextAccessor, UserManager<UserApp> userManager)
        {
            _userRepository = userRepository;
            _newDBContext = newDBContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpUser user)
        {
            var result = await _userRepository.SignUpAsync(user);
            if (result == "true")
            {
                return Ok();
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
            var result = await _userRepository.SignUpUserAsync(user);
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
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest();
            }
            
            var auth = new AuthenRespone{
                User = user.Email,
                Token = result
            };
            return Ok(auth);
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetProfileAsync([FromBody] String Email)
        {
            var user = await _userRepository.GetProfileUser(Email);
            if (user == null)
            {
                return BadRequest();
            }
            return Ok(user);
        }

        [HttpGet("test")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetCurrentID()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);
            return Ok(email);
        }
    }
}
