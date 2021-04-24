using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Notify.Workers
{
    public class NotifyWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotifyWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cache = scope.ServiceProvider.GetService<NotifyCache>()!;
            var bot = scope.ServiceProvider.GetService<TelegramBotClient>()!;

            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = cache.ByDate
                    .Where(x => Math.Abs((x.Key - DateTimeOffset.Now).TotalMinutes) < 2)
                    .ToImmutableArray();

                notifications.AsParallel().ForAll(x =>
                {
                    foreach (var notifyModel in x.Value)
                    {
                        bot.SendTextMessageAsync(notifyModel.ChatId, notifyModel.Name + notifyModel.Date);
                        cache.ByDate.TryRemove(new KeyValuePair<DateTimeOffset, IEnumerable<NotifyModel>>(x.Key, x.Value));
                    }
                });

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}
