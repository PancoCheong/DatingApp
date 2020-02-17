namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // like other user
        public int LikeeId { get; set; } //being like by other
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}