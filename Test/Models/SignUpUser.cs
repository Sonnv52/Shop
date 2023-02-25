namespace Test.Models
{
    public class SignUpUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; } = string.Empty;
        public string PasswordConfirmed { get; set; }
        public string Adress { get; set; }
        public SignUpUser() { 
        
        }
    }
}
