using Microsoft.AspNetCore.Identity;

namespace gestionale_scalzo.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool Revoked { get; set; }
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}
