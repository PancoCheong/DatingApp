namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // like other user
        public int LikeeId { get; set; } //being like by other        
        // virtual for lazy loading
        public virtual User Liker { get; set; }
        public virtual User Likee { get; set; }
    }
}