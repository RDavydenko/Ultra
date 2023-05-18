using Microsoft.EntityFrameworkCore;

namespace Ultra.Auth.WebApi.DAL
{
    public class AuthDbContext : DbContext
    {
        public string Schema => "ult_auth";

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
