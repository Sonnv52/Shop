using Shop.Api.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Api.Data
{
    [Table("BillDetail")]
    public class BillDetail
    {
        [Key]
        public Guid Id { get; set; }
        public int Totals { get; set; }
        public string? Size { get; set; }
        public Bill? Bill { get; set; }
        public Product? Product { get; set; }
        public double? Price { get; set; }
    }
}
