using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Data
{
    [Table("BillDetail")]
    public class BillDetail
    {
        [Key]

        public Guid Id { get; set; }
        public IList<Bill> Bills { get; set; }
        public IList<Product> Products { get; set; }
        public int Totals { get; set; }
    }
}
