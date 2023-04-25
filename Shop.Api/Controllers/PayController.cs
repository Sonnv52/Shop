using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Shop.Api.Abtracst;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPayService _payService;
        public PayController(IPayService payService, IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _payService = payService;
        }
        [HttpPost]
        [Route("/Pay")]
        public async Task<IActionResult> PayAsync()
        {
            //var user = HttpContext.Items["User"] as UserApp;
            var result = await _payService.GetUrlPayAsync(1000);
            return Ok(result);
        }
    }
}
