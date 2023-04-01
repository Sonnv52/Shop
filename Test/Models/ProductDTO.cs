namespace Shop.Api.Models
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public double? Seleoff { get; set; }
        public byte[]? IM { get; set; }
        public IList<SizeDTO>? Sizes { get; set; }
      /*  public ProductDTO(Guid id, string? name, double price, string? description, string? image, double? seleoff, byte[]? iM, IList<SizeDTO>? sizes) => (Id, Name, Price, Description, 
            Image, Seleoff, IM, Sizes) = (id, name, price, description, image, seleoff, iM, sizes );*/
       
    }
}
