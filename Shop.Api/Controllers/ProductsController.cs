using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using Shop.Api.Abtracst;
using Shop.Api.Models;
using Shop.Api.Models.CreateModel;
using StackExchange.Redis;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Repository;

namespace Shop.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly NewDBContext _context;
        private readonly IProductServices _productservices;
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(NewDBContext context, IProductServices productservice, IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _productservices = productservice;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Products
        [HttpGet("GetProduct")]

        public async Task<ActionResult<PageProduct>> GetProducts([FromQuery] SearchModel? search)
        {
            /*try
            {
                var cacheKey = $"products:{search?.key}:{search?.sort}:{search?.from}:{search?.from}:{search?.PageSize}:{search?.PageIndex}";
                // Check if the search query is already cached in Redis
                var cachedResult = await _cache.GetStringAsync(cacheKey);
                if (cachedResult != null)
                {
                    // If the result is cached, return it from the cache
                    return Ok(JsonConvert.DeserializeObject<PageProduct>(cachedResult));
                }
                // If the result is not cached, execute the search query
                var result = await _productservices.GetProductAsync(search!);

                // Serialize the result and cache it in Redis for 1 hour
                var serializedResult = JsonConvert.SerializeObject(result);

                await _cache.SetStringAsync(cacheKey, serializedResult, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),

                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }*/
            var results = await _productservices.GetProductAsync(search!);
            // Return the result
            return Ok(results);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(Guid id)
        {
#pragma warning disable CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (id == null) throw new ArgumentNullException("id");
#pragma warning restore CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
            var result = await _productservices.GetOneProductAsync(id);
            return result;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        // DELETE: api/Products/5
        [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

           product.IsDeleted= true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer"), Authorize(Roles = "Admin")]
        [HttpDelete("CencelDelete/{id}")]
        public async Task<IActionResult> CencelDeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.IsDeleted = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

    }
}
