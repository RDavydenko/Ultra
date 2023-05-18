using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Msg.WebApi.DAL.Configurations
{
    public class ChannelConfigurator : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.ToTable("CHANNEL");
            builder.HasIdColumn();

            builder.Property(x => x.Type).HasColumnName("TYPE").HasEnumAsStringConversion().IsRequired();
            builder.Property(x => x.Name).HasColumnName("NAME").IsRequired();
            builder.Property(x => x.CreateDate).HasColumnName("CREATE_DATE");
            builder.Property(x => x.CreateUserId).HasColumnName("CREATE_USER_ID").IsRequired();

            builder.HasMany(x => x.Messages).WithOne(x => x.Channel).HasForeignKey(x => x.ChannelId);
            builder.HasMany(x => x.Users).WithOne(x => x.Channel).HasForeignKey(x => x.ChannelId);
        }
    }
}
