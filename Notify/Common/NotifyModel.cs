using System;

namespace Notify.Common
{
    public class NotifyModel
    {
        public long ChatId { get; set; }
        public Guid NotifyId { get; set; }
        public DateTimeOffset Date { get; set; }
        public NotifyState CurrentState { get; set; }
        public string Name { get; set; } = null!;

        public override string ToString() => string.Join(" | ", ChatId, NotifyId, Name, Date);

        public static NotifyModel FromString(string modelString)
        {
            var properties = modelString.Split('|');

            return new NotifyModel
            {
                ChatId = long.TryParse(properties[0], out var chatId)
                    ? chatId
                    : throw new FormatException($"Мы как-то записали неверный формат {nameof(ChatId)}"),
                NotifyId = Guid.TryParse(properties[1], out var notifyId) 
                    ? notifyId 
                    : throw new FormatException($"Мы как-то записали неверный формат {nameof(NotifyId)}"),
                Name = properties[2],
                Date = DateTimeOffset.TryParse(properties[3], out var date)
                    ? date
                    : throw new FormatException($"Мы как-то записали неверный формат {nameof(Date)}"),
            };
        }
    }
}
