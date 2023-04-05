using Microsoft.AspNetCore.Identity;

namespace Test.Data
{
    public class UserApp : IdentityUser
    {
        public string? Name { get; set; }
        public string? Adress { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<Bill>? Bills { get; set; }
    }
}
