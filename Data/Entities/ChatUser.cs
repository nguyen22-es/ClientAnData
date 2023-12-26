using DataAccess.Entities;

namespace SocialNetworking.Data.Entities
{
    public class ChatUser
    {
        public string ChatUserID { get; set; }
        public string IDuser1 { get; set; }
        public string IDuser2 { get; set; } 
        



        public   User User1 { get; set; }
        public  User User2 { get; set; }
        public ICollection<Messages> Messages { get; set; }
    
    }
}
