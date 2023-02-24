using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Data
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double Seleoff { get; set; } = 0!;
        public virtual ICollection<BillDetail> BillDetails { get;set; }
        public Type Type { get; set; }
        public Product() { 
            BillDetails = new List<BillDetail>();
        }
    }
}
