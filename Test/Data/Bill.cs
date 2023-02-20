using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Data
{
    [Table("Bill")]   
    
    public class Bill
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime OderDate { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public Customer Customer { get; set; }
    }
}
