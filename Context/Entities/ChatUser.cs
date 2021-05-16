using System.ComponentModel.DataAnnotations;

namespace Context.Entities
{
    public class ChatUser
    {
        [Key]
        public long ChatId { get; set; }
    }
}
