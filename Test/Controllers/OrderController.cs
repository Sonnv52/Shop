using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Shop.Api.Models;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Order;
using System.Net.WebSockets;
using Test.Data;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly NewDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public OrderController(NewDBContext dBContext, IConfiguration configuration)
        {
            _dbContext = dBContext;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> OrderAsync([FromBody] OrderRequest request)
        {
            return Ok();
        }

        [HttpGet]
        [Route("getimage")]
        public async Task<IActionResult> GetImageAsync()
        {
            return Ok();
        }
        [HttpPost("Post")]
        public async Task<IActionResult> SaveImage([FromForm] ProductAdd product)
        {
            BlobContainerClient blod = new BlobContainerClient(_configuration["AzureString"], "shoimage");
            using(var stream = new MemoryStream())
            {
                await product.Image.CopyToAsync(stream);
                stream.Position = 0;
                await blod.UploadBlobAsync(product.Image.FileName, stream);
            }
            return Ok();
        }
        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            BlobContainerClient blod = new BlobContainerClient(_configuration["AzureString"], "shoimage");
            BlobClient blobClient = blod.GetBlobClient(fileName);
            var response = await blobClient.DownloadAsync();

            if (response.Value.ContentLength == 0)
            {
                return NotFound();
            }

            Stream stream = new MemoryStream();
            await response.Value.Content.CopyToAsync(stream);
            stream.Position = 0;

            return File(stream, response.Value.ContentType);
        }
    }
}
