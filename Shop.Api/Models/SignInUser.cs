using System.ComponentModel.DataAnnotations;

namespace Shop.Api.Models
{
    public class SignInUser
    {
        [Required, EmailAddress(ErrorMessage ="Email is required")]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
