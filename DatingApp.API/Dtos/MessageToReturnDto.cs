using System;

namespace DatingApp.API.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }
        // AutoMapper knows this map to Sender.Id
        public int SenderId { get; set; }
        // AutoMapper knows this map to Sender.KnownAs
        public string SenderKnownAs { get; set; } // use string instead of User Object
        public string SenderPhotoUrl { get; set; } // also send back main Photo Url
        // AutoMapper knows this map to Recipient.Id
        public int RecipientId { get; set; }
        // AutoMapper knows this map to Recipient.KnownAs
        public string RecipientKnownAs { get; set; } // use string instead of User Object
        public string RecipientPhotoUrl { get; set; } // also send back main Photo Url
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}