using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Notify
{
    public class NotifyCache
    {
        private readonly ILogger<NotifyCache> _logger;

        public readonly ConcurrentDictionary<long, NotifyModel> ByUser =
            new ConcurrentDictionary<long, NotifyModel>();

        public readonly ConcurrentDictionary<DateTimeOffset, NotifyModel> ByDate =
            new ConcurrentDictionary<DateTimeOffset, NotifyModel>();

        public NotifyCache(ILogger<NotifyCache> logger)
        {
            _logger = logger;
        }

    }

    public class NotifyModel
    {
        public long ChatId { get; set; }
        public DateTimeOffset Date { get; set; }
        public NotifyState State { get; set; }
        public string Name { get; set; } = null!;

        public NotifyModel Update(string message)
        {
            switch (State)
            {
                case NotifyState.Empty: Name = message; break;
                case NotifyState.Name: Date = DateTimeOffset.TryParse(message, out var date) 
                    ? date 
                    : throw new ArgumentException("Неверный формат даты");
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
