namespace Shop.Api.Models
{
    public class ResetPasswordModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
       // public string? ConfirmPassword { get; set;}
        public string? TokenReset { get; set;}
    }
}
