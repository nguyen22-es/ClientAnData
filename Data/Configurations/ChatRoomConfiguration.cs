using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetworking.Data.Entities;
using DATAA.Data.Entities;

namespace DATAA.Data.Configurations
{
    public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
    {
        public void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            builder.HasKey(r => r.RoomID);
            builder.Property(m => m.RoomID)
             .ValueGeneratedOnAdd();

            builder.Property(m => m.NameRoom)
             .IsRequired(false);

            builder.Property(m => m.Admin)
                .IsRequired();


            builder.HasOne(m => m.User)
                .WithMany(m => m.chatRooms)
                .HasForeignKey(m => m.Admin)
                  .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

