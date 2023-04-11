using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Shop.Api.Data
{
    [Table("Bill")]   
    
    public class Bill
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime OderDate { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = "Đã đặt hàng";
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        
        public UserApp UserApp { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Bill() { 
          BillDetails = new HashSet<BillDetail>();        
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
