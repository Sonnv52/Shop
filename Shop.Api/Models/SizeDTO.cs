using Microsoft.Build.Framework;
using Shop.Api.Data;

namespace Shop.Api.Models
{
    public class SizeDTO
    {

        public Guid IdSizelog { get; set; }
        public string? size { get; set; }
        public int Qty { get; set; }
    }
}
