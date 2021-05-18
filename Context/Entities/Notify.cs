using Common.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Context.Entities
{
    public class Notify
    {
        [Key]
        public Guid NotifyId { get; set; }

        [ForeignKey("UserId")]
        public virtual ChatUser ChatUser { get; set; } = null!;
        public long UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        public FrequencyType Frequency { get; set; }
        public string Name { get; set; } = null!;
        public TimeSpan FrequencyTime { get; set; }
    }
}
