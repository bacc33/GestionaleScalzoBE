using gestionale_scalzo.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;   
using Microsoft.EntityFrameworkCore;

namespace gestionale_scalzo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Order> Orders { get; set; }
        public DbSet<Tipologia> Tipologie{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
