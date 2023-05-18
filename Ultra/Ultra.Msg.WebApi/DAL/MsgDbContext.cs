using Microsoft.EntityFrameworkCore;

namespace Ultra.Msg.WebApi.DAL
{
    public class MsgDbContext : DbContext
    {
        public string Schema => "ult_msg";

        public MsgDbContext(DbContextOptions<MsgDbContext> options)
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
