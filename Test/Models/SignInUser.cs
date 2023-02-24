using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class SignInUser
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
