using DataAccess.Entities;

namespace DATAA.Data.Entities
{
    public class ChatRoom
    {

        public string RoomID { get; set; }

        public string NameRoom { get; set; }

        public string Admin { get; set; }
        public User User { get; set; }
        public ICollection<Messages> Messages { get; set; }
    }
}
