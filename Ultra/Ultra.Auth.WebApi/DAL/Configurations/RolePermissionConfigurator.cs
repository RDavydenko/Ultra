using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Auth.WebApi.DAL.Entities;

namespace Ultra.Auth.WebApi.DAL.Configurations;

public class RolePermissionConfigurator : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("ROLE_PERMISSION");
        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.Property(x => x.PermissionId).HasColumnName("PERMISSION_ID");
        builder.Property(x => x.RoleId).HasColumnName("ROLE_ID");
        
        builder.HasOne(x => x.Permission).WithMany(x => x.Roles).HasForeignKey(x => x.PermissionId);
        builder.HasOne(x => x.Role).WithMany(x => x.Permissions).HasForeignKey(x => x.RoleId);
    }
}
