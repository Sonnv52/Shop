using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shop.Api.Data;

namespace Shop.Api.Data
{
    [Table("ImageProducts")]
    public class ImageProducts
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        public string? ImagePath { get; set; }

        public string? Caption { get; set; }

        public bool IsDefault { get; set; }

        public DateTime DateCreated { get; set; }

        public int SortOrder { get; set; }

        public long FileSize { get; set; }

        public Product? Product { get; set; }
    }
}
