﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Abtracst;
using Shop.Api.Data;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Order;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Cryptography;
using MassTransit;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderServices _orderServices;
        private readonly IBus _bus;
        public OrderController(IBus bus, IConfiguration configuration, IOrderServices orderServices)
        {
            _configuration = configuration;
            _orderServices = orderServices;
            _bus= bus;
        }

        [HttpPost]
        [Route("Checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> OrderAsync([FromBody] OrderRequest request, CancellationToken token)
        {
            string emailUser = User.FindFirstValue(ClaimTypes.Name);
            var result = await _orderServices.OrderAsync(request, emailUser, token);
            if (result.Status)
            {
                return Ok(result);
            }
            return StatusCode(430, result.Message);
        }

        [HttpPost]
        [Route("/GetTotal")]
        public async Task<IActionResult> GetTotalAsync(IList<ProductsRequest?> products)
        {
            if (products is null)
            {
                return BadRequest();
            }
            var result = await _orderServices.GetPriceAsync(products);
            return Ok(result);
        }

        [HttpPost("Post")]
        public async Task<IActionResult> SaveImage([FromForm] ProductAddModel product)
        {
            BlobContainerClient blod = new BlobContainerClient(_configuration["AzureString"], "shoimage");
            using (var stream = new MemoryStream())
            {
                if(product.Image == null) { return BadRequest(product.Name); }
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
            var result = await _orderServices.GetYourBillAsync(email);
            return Ok(result);
        }

        [HttpGet]
        [Route("OrderDetail")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetBillDetailAsync(Guid id)
        {
            string email = User.FindFirstValue(ClaimTypes.Name);
            var result = await _orderServices.GetYourBillDetaillAsync(email,id);
            return Ok(result);
        }

        [HttpPatch]
        [Route("/CancelBill")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CancelBillAsync(Guid id)
        {
            var result = true;
            return Ok(result);
        }
    }
}
    

