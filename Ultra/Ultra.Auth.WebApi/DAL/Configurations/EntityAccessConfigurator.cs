using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Auth.WebApi.DAL.Configurations;

public class EntityAccessConfigurator : IEntityTypeConfiguration<EntityAccess>
{
    public void Configure(EntityTypeBuilder<EntityAccess> builder)
    {
        builder.ToTable("ENTITY_ACCESS");
        builder.HasIdColumn();

        builder.Property(x => x.Entity).HasColumnName("ENTITY");
        builder.Property(x => x.EntityId).HasColumnName("ENTITY_ID");
        builder.Property(x => x.Method).HasColumnName("METHOD").HasEnumAsStringConversion();
        builder.Property(x => x.Type).HasColumnName("TYPE").HasEnumAsStringConversion();
        builder.Property(x => x.UserId).HasColumnName("USER_ID");
        builder.Property(x => x.RoleId).HasColumnName("ROLE_ID");
        
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
    }
}
