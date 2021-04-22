using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Notify
{
    public class NotifyCache
    {
        private readonly ILogger<NotifyCache> _logger;

        public readonly ConcurrentDictionary<long, List<NotifyModel>> ByUser =
            new ConcurrentDictionary<long, List<NotifyModel>>();

        public readonly ConcurrentDictionary<DateTimeOffset, IEnumerable<NotifyModel>> ByDate =
            new ConcurrentDictionary<DateTimeOffset, IEnumerable<NotifyModel>>();

        public readonly ConcurrentDictionary<long, NotifyModel> InProgressNotifications =
            new ConcurrentDictionary<long, NotifyModel>();

        public NotifyCache(ILogger<NotifyCache> logger)
        {
            _logger = logger;
        }

        public bool TryAdd(long chatId, NotifyModel model)
        {
            var result = InProgressNotifications.TryAdd(chatId, model);

            if (ByUser.ContainsKey(chatId))
            {
                ByUser[chatId].Add(model);
            }
            else
            {
                result = ByUser.TryAdd(chatId, new List<NotifyModel> { model });
            }

            return result;
        }

    }

    public class NotifyModel
    {
        public long ChatId { get; set; }
        public Guid NotifyId { get; set; }
        public DateTimeOffset Date { get; set; }
        public NotifyState State { get; set; }
        public string Name { get; set; } = null!;

        public NotifyModel Update(string message)
        {
            switch (State)
            {
                case NotifyState.Empty:
                    Name = message; break;
                case NotifyState.Name:
                    Date = DateTimeOffset.TryParse(message, out var date) ? date : throw new FormatException("Неверный формат даты");
                    break;
            }

            UpdateState();
            return this;
        }

        public string GetNextStepMessage() => State switch
        {
            NotifyState.Empty => "Введите повод",
            NotifyState.Name => "Введите дату и время в формате 01.01.2021 00:00",
            NotifyState.Date => "Готово",
            NotifyState.Ready => "",
            _ => throw new ArgumentOutOfRangeException()
        };

        public override string ToString() => 
            string.Join('|', Name, Date);

        private void UpdateState()
        {
            State = State switch
            {
                NotifyState.Empty => NotifyState.Name,
                NotifyState.Name => NotifyState.Date,
                NotifyState.Date => NotifyState.Ready,
                _ => State
            };
        }
    }
}
