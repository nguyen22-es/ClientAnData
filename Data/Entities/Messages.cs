using DATAA.Data.Entities;
using SocialNetworking.Data.Entities;

namespace DataAccess.Entities
{
    public class Messages
    {
        public string MessageID { get; set; }

        public string  SenderUserID { get; set; }
        public string chatRoomID { get; set; }
        public string chatUserID { get; set;}
        public DateTime TimeSend { get; set;}
        public string Content { get; set; }



        public User UserSend { get; set; }
        public ChatUser  chatUser { get; set; }
        public ChatRoom chatRoom { get; }
    
    }
}
