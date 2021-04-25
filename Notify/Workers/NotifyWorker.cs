using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Services;
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
            using var scope = _serviceProvider.CreateScope();
            var cache = scope.ServiceProvider.GetService<NotifyCacheService>()!;
            var bot = scope.ServiceProvider.GetService<TelegramBotClient>()!;

            StartBot(scope, bot);

            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = cache.ByUser
                    .SelectMany(x => x.Value)
                    .Where(x => Math.Abs((x.Date - DateTimeOffset.Now).TotalMinutes) < 2)
                    .ToImmutableArray();

                notifications.AsParallel().ForAll(x =>
                {
                    bot.SendTextMessageAsync(x.ChatId, x.ToString());
                    cache.ByUser[x.ChatId].Remove(x);
                });

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        private static void StartBot(IServiceScope scope, ITelegramBotClient bot)
        {
            var notifyService = scope.ServiceProvider.GetService<NotifyService>();

            bot!.OnMessage += notifyService!.OnMessage;
            bot.OnCallbackQuery += notifyService!.OnOnCallbackQuery;

            bot.StartReceiving();
        }
    }
}
