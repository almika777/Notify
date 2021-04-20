using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Notify
{
    public class NotifyCache
    {
        private readonly ILogger<NotifyCache> _logger;

        public readonly ConcurrentDictionary<DateTimeOffset, NotifyModel> CacheByDate =
            new ConcurrentDictionary<DateTimeOffset, NotifyModel>();

        public readonly ConcurrentDictionary<long, IEnumerable<NotifyModel>> CacheByUser =
            new ConcurrentDictionary<long, IEnumerable<NotifyModel>>();

        public readonly ConcurrentDictionary<long, NotifyModel> UserCurrentNotify =
            new ConcurrentDictionary<long, NotifyModel>();

        public NotifyCache(ILogger<NotifyCache> logger)
        {
            _logger = logger;
        }

    }

    public class NotifyModel
    {
        public DateTimeOffset Date { get; set; }
        public NotifyState State { get; set; }
        public string Name { get; set; } = null!;

        public NotifyModel Update(string message)
        {
            UpdateState();
            switch (State)
            {
                case NotifyState.Name:
                    Name = message;
                    break;
                case NotifyState.Date:
                    Date = DateTimeOffset.Parse(message);
                    break;
            }

            return this;
        }

        public string GetNextStepMessage() => State switch
        {
            NotifyState.Empty => "Введите повод",
            NotifyState.Name => "Введите дату",
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
