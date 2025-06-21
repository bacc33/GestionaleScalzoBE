using Microsoft.AspNetCore.Identity;

namespace gestionale_scalzo.Model
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
        public bool FirstAccess { get; set; }
        public bool LastAccess { get; set; }
        public bool Attivo { get; set; }
    }
}
