using Microsoft.AspNetCore.Identity;

namespace gestionale_scalzo.Model
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string Descrizione { get; set; }
    }
}
