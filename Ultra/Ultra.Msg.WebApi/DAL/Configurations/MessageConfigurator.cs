using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;

namespace Ultra.Msg.WebApi.DAL.Configurations
{
    public class MessageConfigurator : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("MESSAGE");
            builder.HasGuidColumn();

            builder.Property(x => x.ChannelId).HasColumnName("CHANNEL_ID").IsRequired();
            builder.Property(x => x.SendUserId).HasColumnName("SEND_USER_ID").IsRequired();
            builder.Property(x => x.SendDate).HasColumnName("SEND_DATE");
            builder.Property(x => x.Text).HasColumnName("TEXT").IsRequired();

            builder.HasOne(x => x.Channel).WithMany(x => x.Messages).HasForeignKey(x => x.ChannelId);
        }
    }
}
