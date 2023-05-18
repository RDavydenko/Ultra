using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ultra.Msg.WebApi.DAL.Entities;

namespace Ultra.Msg.WebApi.DAL.Configurations
{
    public class ChannelUserConfigurator : IEntityTypeConfiguration<ChannelUser>
    {
        public void Configure(EntityTypeBuilder<ChannelUser> builder)
        {
            builder.ToTable("CHANNEL_USER");
            builder.HasKey(x => new { x.ChannelId, x.UserId });

            builder.Property(x => x.ChannelId).HasColumnName("CHANNEL_ID").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("USER_ID").IsRequired();
            builder.Property(x => x.Silenced).HasColumnName("SILENCED");

            builder.HasOne(x => x.Channel).WithMany(x => x.Users).HasForeignKey(x => x.ChannelId);
        }
    }
}
