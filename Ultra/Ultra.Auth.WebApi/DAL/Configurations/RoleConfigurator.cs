using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Auth.WebApi.DAL.Configurations;

public class RoleConfigurator : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("ROLE");

        builder.HasIdColumn();
        builder.Property(x => x.State).HasStateColumn();

        builder.Property(x => x.Code).HasColumnName("CODE");
        builder.Property(x => x.Name).HasColumnName("NAME");
        builder.Property(x => x.Description).HasColumnName("DESCRIPTION");
        
        builder.HasMany(x => x.Users).WithOne(x => x.Role).HasForeignKey(x => x.RoleId);
    }
}
