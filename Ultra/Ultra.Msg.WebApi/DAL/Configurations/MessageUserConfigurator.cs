using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ultra.Msg.WebApi.DAL.Entities;

namespace Ultra.Msg.WebApi.DAL.Configurations
{
    public class MessageUserConfigurator : IEntityTypeConfiguration<MessageUser>
    {
        public void Configure(EntityTypeBuilder<MessageUser> builder)
        {
            builder.ToTable("MESSAGE_USER");
            builder.HasKey(x => new { x.MessageGuid, x.UserId });

            builder.Property(x => x.MessageGuid).HasColumnName("MESSAGE_GUID").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("USER_ID").IsRequired();
            builder.Property(x => x.Received).HasColumnName("RECEIVED");
            builder.Property(x => x.ReceivedDate).HasColumnName("RECEIVED_DATE");
            builder.Property(x => x.Read).HasColumnName("READ");
            builder.Property(x => x.ReadDate).HasColumnName("READ_DATE");

            builder.HasOne(x => x.Message).WithMany(x => x.Users).HasForeignKey(x => x.MessageGuid);
        }
    }
}
