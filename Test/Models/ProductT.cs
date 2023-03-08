using Test.Data;

namespace Test.Models
{
    public class ProductT
    {
        public Guid Id { get; set; }
      
        public string? Name { get; set; }
     
        public double Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public double? Seleoff { get; set; }
       
    }
}
