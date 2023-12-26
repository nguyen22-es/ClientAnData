
using DATAA.Data.Entities;
using Microsoft.AspNetCore.Identity;
using SocialNetworking.Data.Entities;

namespace DataAccess.Entities
{
    public class User 
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string Avatar { get; set; }


        public ICollection<Messages> Messages { get; set; }

        public ICollection<ChatRoom>  chatRooms { get; set; }
        public ICollection<ChatRoomUser> chatRoomUsers  { get; set; }

        public ICollection<ChatUser> chatUser1 { get; set; }
        public ICollection<ChatUser> chatUser2 { get; set; }

    }
}
