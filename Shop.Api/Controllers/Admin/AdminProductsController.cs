using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Abtracst;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Products;
using Shop.Api.Data;
using System.Security.Claims;
using Shop.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Shop.Api.Filter;

namespace Shop.Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly NewDBContext _context;
        private readonly  IProductServices _productServices;

        public AdminProductsController(NewDBContext context, IProductServices productServices)
        {
            _context = context;
            _productServices = productServices;
        }

        // PUT: api/AdminProducts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutProduct( [FromForm] ProductAddModel product)
        {
            var result = await _productServices.SetProductAsync(product);
            if(result) return Ok();
            return StatusCode(450, false);
        }

        [HttpPost("UpLoad/Imgage")]
        public async Task<IActionResult> UploadImageAsync()
        {
            return Ok();
        }
        // POST: api/AdminProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add/Product")]
        // [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FileFormatFilter))]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductAddModel product)
        {
            string result = await _productServices.AddProductAsysnc(product);
            /*try
            {
                var keys = await _cache.GetKeysByPatternAsync($"*{cacheKey}*");
            }
            catch(DbUpdateConcurrencyException)
            {

            }*/
            return Ok(result);
        }

        //Add size for product after add product
        [HttpPut("addSize")]
        public async Task<IActionResult> AddSizeAsync([FromBody] AddSizeModel<StringSize> stringSizes)
        {
            var result = await _productServices.AddSizeProductAsync(stringSizes);
            if(!result.Equals("Suggest!!"))
            {
                return StatusCode(450,result);
            }
            return Ok(result);
        }
        // DELETE: api/AdminProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
