using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Core.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Core.DAL.Configurators
{
    internal class FavoriteItemConfigurator : IEntityTypeConfiguration<FavoriteItem>
    {
        public void Configure(EntityTypeBuilder<FavoriteItem> builder)
        {
            builder.ToTable("FAVORITE_ENTITY");
            builder.HasIdColumn();

            builder.Property(x => x.UserId).HasColumnName("USER_ID");
            builder.Property(x => x.EntityTypeId).HasColumnName("ENTITY_TYPE_ID");
            builder.Property(x => x.EntityId).HasColumnName("ENTITY_ID");

            builder.HasOne(x => x.EntityType).WithMany().HasForeignKey(x => x.EntityTypeId);
        }
    }
}
