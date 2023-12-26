using DATAA.Data.Entities;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetworking.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ManageAppDbContext:DbContext
    {
        public ManageAppDbContext(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }




        public DbSet<Messages> Messages { get; set; }
        public DbSet<ChatRoom> chatRooms { get; set; }
        public DbSet<ChatRoomUser>  chatRoomUsers { get; set; }

        public DbSet<User>  Users { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set;}



    }
}
