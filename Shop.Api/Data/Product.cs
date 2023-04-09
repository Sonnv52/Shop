using Shop.Api.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Api.Data
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double? Seleoff { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get;set; }
        public virtual ICollection<ImageProducts> ImageProducts { get; set; }
        public string? CreateAt { get; set; }  
        public virtual ICollection<Size> Sizes { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Product() {
            ImageProducts = new List<ImageProducts>();
            BillDetails = new List<BillDetail>();
            CreateAt = DateTime.Now.ToString();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
