using DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetworking.Data.Entities;

namespace DATAA.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("UserProfile"); // Đặt tên bảng là "Messages"
            builder.HasKey(m => m.ID);

            builder.Property(m => m.ID)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .IsRequired();

            builder.Property(m => m.Avatar)
               .IsRequired();


        }
    }
}
