﻿using System;

namespace Common.Common
{
    public class NotifyModel
    {
        public long ChatId { get; set; }
        public Guid NotifyId { get; set; }
        public DateTimeOffset Date { get; set; }
        public NotifyStep NextStep { get; set; }
        public string Name { get; set; } = null!;

        /// <summary>
        /// Return NotifyModel as string representation with the order of properties (ChatId, NotifyId, Name, Date)
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join(CommonResource.Separator, ChatId, NotifyId, Name, Date.ToString("g"));

        public override bool Equals(object obj)
        {
            var model = obj as NotifyModel;

            if (model == null) return false;

            return ChatId == model.ChatId && NotifyId == model.NotifyId;
        }


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
            };
        }
    }
}
