using Shop.Api.Data;
using Shop.Api.Models.Products;
using System.ComponentModel.DataAnnotations;

namespace Shop.Api.Models.CreateModel
{
    public class ProductAddModel
    {
        public Guid? id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
