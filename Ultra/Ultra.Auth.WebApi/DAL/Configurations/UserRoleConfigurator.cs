using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Auth.WebApi.DAL.Entities;

namespace Ultra.Auth.WebApi.DAL.Configurations;

public class UserRoleConfigurator : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("USER_ROLE");
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.RoleId).HasColumnName("ROLE_ID");
        
        builder.HasOne(x => x.User).WithMany(x => x.Roles).HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId);
    }
}
