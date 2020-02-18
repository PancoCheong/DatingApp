using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; } // allow null value
        public DateTime MessageSent { get; set; }
        public bool SenderDeleted { get; set; } // delete by Sender
        public bool RecipientDeleted { get; set; } // delete by Reciever
    }
}