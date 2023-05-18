using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ultra.Core.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Core.DAL.Configurators
{
    internal class EntityTypeConfigurator : IEntityTypeConfiguration<EntityType>
    {
        public void Configure(EntityTypeBuilder<EntityType> builder)
        {
            builder.ToTable("ENTITY_TYPE");
            builder.HasIdColumn();

            builder.Property(x => x.SystemName).HasColumnName("SYSTEM_NAME");
            builder.Property(x => x.DisplayName).HasColumnName("DISPLAY_NAME");
        }
    }
}
