using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class ChatUser
    {
        [Key]
        public long ChatId { get; set; }
    }
}
