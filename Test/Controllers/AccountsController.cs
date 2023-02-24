using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;
using Test.Repository;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {   
        private readonly IUserRepository _userRepository;
        private readonly NewDBContext _newDBContext;
        public AccountsController(IUserRepository userRepository, NewDBContext newDBContext)
        {
            _userRepository = userRepository;
            _newDBContext = newDBContext;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(SignUpUser user)
        {
            var result = await _userRepository.SignUpAsync(user);
            if (result == "true")
            {
                return Ok();
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
            var x = new AuthenRespone{
                User = user.Email,
                Token = result
            };
            return Ok(x);
        }
        [Authorize]
        [HttpGet("Product")]
        
        public async Task<IActionResult> TestAysnc()
        {
            var result = _newDBContext.Products.FirstOrDefault();
            return Ok(result);
        }
    }
}
