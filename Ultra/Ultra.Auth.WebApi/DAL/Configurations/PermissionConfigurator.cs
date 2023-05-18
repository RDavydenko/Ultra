using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Auth.WebApi.DAL.Configurations;

public class PermissionConfigurator : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("PERMISSION");

        builder.HasIdColumn();
        builder.Property(x => x.State).HasStateColumn();

        builder.Property(x => x.Entity).HasColumnName("ENTITY");
        builder.Property(x => x.Action).HasColumnName("ACTION");
        builder.Property(x => x.Description).HasColumnName("DESCRIPTION");
        
        builder.HasMany(x => x.Roles).WithOne(x => x.Permission).HasForeignKey(x => x.RoleId);
    }
}
