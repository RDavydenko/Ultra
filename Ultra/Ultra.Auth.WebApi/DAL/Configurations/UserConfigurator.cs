using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Auth.WebApi.DAL.Configurations;

public class UserConfigurator : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USER");

        builder.HasIdColumn();
        builder.Property(x => x.State).HasStateColumn();

        builder.Property(x => x.Login).HasColumnName("LOGIN");
        builder.Property(x => x.UserName).HasColumnName("USER_NAME");
        builder.Property(x => x.PasswordHash).HasColumnName("PASSWORD_HASH");
        builder.Property(x => x.Salt).HasColumnName("SALT");
        
        builder.HasMany(x => x.Roles).WithOne(x => x.User).HasForeignKey(x => x.UserId);
    }
}
