using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Services;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var cache = _serviceProvider.GetService<NotifyCacheService>()!;
            var bot = _serviceProvider.GetService<TelegramBotClient>()!;

            await cache.Initialize();
            StartBot(bot);
            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = cache.ByUser
                    .SelectMany(x => x.Value)
                    .Where(x => Math.Abs((x.Value.Date - DateTimeOffset.Now).TotalMinutes) < 2)
                    .ToImmutableArray();

                notifications.AsParallel().ForAll(x =>
                {
                    bot.SendTextMessageAsync(x.Value.ChatId, x.ToString());
                    cache.ByUser[x.Value.ChatId].Remove(x);
                });

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        private void StartBot(ITelegramBotClient bot)
        {
            using var scope = _serviceProvider.CreateScope();
            var notifyService = scope.ServiceProvider.GetService<NotifyService>();

            bot!.OnMessage += notifyService!.OnMessage;
            bot.OnCallbackQuery += notifyService!.OnCallbackQuery;
            bot.StartReceiving();
        }
    }
}
