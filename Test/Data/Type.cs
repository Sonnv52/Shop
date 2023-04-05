using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Data
{
    [Table("Type")]   
    public class Type
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Type()
        {
            Name= string.Empty;
            Products = new HashSet<Product>();
        }
    }
}
