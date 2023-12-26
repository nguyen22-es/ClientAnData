using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetworking.Data.Entities;
using DATAA.Data.Entities;

namespace DATAA.Data.Configurations
{
    public class ChatRoomUserConfiguration : IEntityTypeConfiguration<ChatRoomUser>
    {
        public void Configure(EntityTypeBuilder<ChatRoomUser> builder)
        {
            //builder.ToTable("LikePosts");
           // builder.HasKey(lp => new { lp.PostId, lp.UserId });
            builder.HasKey(r => new {r.UserID,r.RoomID});
        
            builder.Property(m => m.UserID)
               .IsRequired();

            builder.Property(m => m.RoomID)
               .IsRequired();


            builder.HasOne(r => r.user)
                .WithMany(u => u.chatRoomUsers)
                .HasForeignKey(r => r.UserID)
                  .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(r => r.room)
                .WithMany()
                .HasForeignKey(r => r.RoomID)
                  .OnDelete(DeleteBehavior.NoAction);



        }
    


    }
}
