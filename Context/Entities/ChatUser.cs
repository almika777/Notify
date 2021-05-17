using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Context.Entities
{
    public class ChatUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ChatId { get; set; }
    }
}
