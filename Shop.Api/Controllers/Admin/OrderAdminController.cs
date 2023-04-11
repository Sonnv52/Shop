using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAdminController : ControllerBase
    {
        [Route("/SetStatus")]
        [HttpPatch]
        public async Task<IActionResult>  SetStatusAsync(string status, Guid Id)
        {

        }
    }
}
