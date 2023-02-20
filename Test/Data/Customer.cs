using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Data
{
    [Table("Customer")]   
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
    }
}
