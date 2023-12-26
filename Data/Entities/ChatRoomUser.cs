using DataAccess.Entities;

namespace DATAA.Data.Entities
{
    public class ChatRoomUser
    {
        public string UserID { get; set; }

        public string RoomID { get; set; }


        public User user {get;set;}

        public ChatRoom room { get; set;}


    }
}
