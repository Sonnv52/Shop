using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Shop.Api.Abtracst;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Order;
using Shop.Api.Models.Products;
using StackExchange.Redis;
using System.Net.WebSockets;
using System.Text.Json.Nodes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Shop.Api.Data;
using System.Text;
using System.Security.Cryptography;
using MassTransit;
using MassTransit.Transports;
using Shop.Api.Models;
using Share.Message;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly NewDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IOrderServices _orderServices;
        private readonly IBus _bus;
        public OrderController(IBus bus, NewDBContext dBContext, IConfiguration configuration, IOrderServices orderServices)
        {
            _dbContext = dBContext;
            _configuration = configuration;
            _orderServices = orderServices;
            _bus= bus;
        }

        [HttpPost]
        [Route("Checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> OrderAsync([FromBody] OrderRequest request)
        {
            string emailUser = User.FindFirstValue(ClaimTypes.Name);
            var result = await _orderServices.OrderAsync(request, emailUser);
            if (result.Status)
            {
                return Ok(result);
            }
            return StatusCode(430, result.Message);
        }

        [HttpPost]
        [Route("GetTotal")]
        public async Task<IActionResult> GetTotalAsync(IList<ProductsRequest?> products)
        {
            if (products == null)
            {
                return BadRequest();
            }
            var result = await _orderServices.GetPriceAsync(products);
            return Ok(result);
        }

        [HttpPost("Post")]
        public async Task<IActionResult> SaveImage([FromForm] ProductAdd product)
        {
            BlobContainerClient blod = new BlobContainerClient(_configuration["AzureString"], "shoimage");
            using (var stream = new MemoryStream())
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

        [HttpPost]
        public async Task<string> EncryptAsync(string Name)
        {
            string decryptedUsername = await Task.Run(()=> DecryptString(Name, "1111111111111111"));
            return decryptedUsername ?? " ";
        }
        //Decrypt user
        public static string DecryptString(string cipherText, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16];
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    byte[] decryptedBytes = ms.ToArray();
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        [HttpGet]
        [Route("/OrderList")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetBillAsync()
        {
            string email = User.FindFirstValue(ClaimTypes.Name);
            var result = await _orderServices.GetBillsAsync(email);
            return Ok(result);
        }
    }
}
    

