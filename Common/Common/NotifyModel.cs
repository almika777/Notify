using Common.Common.Enum;
using Common.Extensions;
using System;
using System.Text;

#pragma warning disable 659

namespace Common.Common
{
    public class NotifyModel
    {
        public long ChatId { get; set; }
        public Guid NotifyId { get; set; }
        public DateTimeOffset Date { get; set; }
        public NotifyStep NextStep { get; set; }
        public FrequencyType Frequency { get; set; }
        public string Name { get; set; } = null!;
        public TimeSpan FrequencyTime { get; set; }

        public void ChangeName(string newName) => Name = newName;
        public void ChangeDate(DateTimeOffset date) => Date = date;

        public string GetNextStepMessage() => NextStep switch
        {
            NotifyStep.Date => "Введите дату и время в формате 01.01.2021 00:00",
            NotifyStep.Frequency => "Выберите периодичность",
            NotifyStep.Ready => "Готово",
            _ => throw new ArgumentOutOfRangeException()
        };

        public static NotifyModel FromString(string modelString)
        {
            var properties = modelString.Split(CommonResource.Separator);

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
                Frequency = int.TryParse(properties[4], out var frequency)
                    ? (FrequencyType)frequency
                    : throw new FormatException($"Мы как-то записали неверный формат {nameof(Frequency)}"),
            };
        }

        /// <summary>
        /// Return NotifyModel as string representation with the order of properties (ChatId, NotifyId, Name, Date, Frequency)
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join(CommonResource.Separator, ChatId, NotifyId, Name, Date.ToString("g"), (int)Frequency);

        public string ToTelegramChat()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"<b>Что</b>: {Name}</n>");
            sb.AppendLine($@"<b>Когда</b>: {Date:g}");
            sb.AppendLine($@"<b>Как часто</b>: {Frequency.GetDescription()}");
            return sb.ToString();
        }
        public override bool Equals(object obj)
        {
            var model = obj as NotifyModel;

            if (model == null) return false;

            return ChatId == model.ChatId && NotifyId == model.NotifyId;
        }
    }
}
