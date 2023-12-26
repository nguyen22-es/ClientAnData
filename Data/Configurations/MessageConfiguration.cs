using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DataAccess.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Messages>
    {
        public void Configure(EntityTypeBuilder<Messages> builder)
        {
            builder.ToTable("Messages"); // Đặt tên bảng là "Messages"
            builder.HasKey(m => m.MessageID);

            builder.Property(m => m.MessageID)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.TimeSend)
                .IsRequired();
            builder.Property(m => m.SenderUserID)
              .IsRequired();
            builder.Property(m => m.chatUserID)
               .IsRequired(false);
            builder.Property(m => m.chatRoomID)
              .IsRequired(false);

            builder.HasOne(m => m.UserSend)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderUserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.chatUser)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.chatUserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.chatRoom)
              .WithMany(c => c.Messages)
              .HasForeignKey(m => m.chatRoomID)
              .IsRequired()
              .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
