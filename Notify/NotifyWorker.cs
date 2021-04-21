using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Notify
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
                    bot.SendTextMessageAsync(x.Value.ChatId, x.Value.Name + x.Value.Date.Date.ToShortDateString());
                    cache.ByDate.TryRemove(new KeyValuePair<DateTimeOffset, NotifyModel>(x.Key, x.Value));
                });

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}
