using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Models.CreateModel;
using Test.Data;
using Test.Repository;

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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AdminProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        [HttpPost("Add/Product")]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductAdd product)
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


        // DELETE: api/AdminProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
