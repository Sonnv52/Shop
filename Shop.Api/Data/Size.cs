using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shop.Api.Data;

namespace Shop.Api.Data
{
    [Table("Size")]
    public class Size
    {
        [Key]
        public Guid IdSizelog { get; set; }
        [Required(ErrorMessage ="Don't know")]
        public string? size { get; set; }    
        public int Qty { get; set; }
        public virtual Product Products { get; set; } = new Product();
    }
}
