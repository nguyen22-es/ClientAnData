using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetworking.Data.Entities;
using System;

public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.HasKey(r => r.ChatUserID);
        builder.Property(m => m.ChatUserID)
            .ValueGeneratedOnAdd();
        builder.Property(m => m.IDuser1)
           .IsRequired();

        builder.Property(m => m.IDuser2)
           .IsRequired();


        builder.HasOne(r => r.User1)
            .WithMany(r => r.chatUser1)
            .HasForeignKey(r => r.IDuser1)
              .OnDelete(DeleteBehavior.NoAction);
     

        builder.HasOne(r => r.User2)
            .WithMany(r => r.chatUser2)
            .HasForeignKey(r => r.IDuser2)
              .OnDelete(DeleteBehavior.NoAction);



    }
}