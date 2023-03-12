using Shop.Api.Data;
using Test.Data;

namespace Shop.Api.Models.CreateModel
{
    public class ProductAdd
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public int type { get; set; } = 0;
    }
}
