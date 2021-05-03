using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Services;
using Services.Services.IoServices;
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
            var remover = scope.ServiceProvider.GetService<INotifyRemover>()!;

            await cache.Initialize();
            StartBot(scope, bot);

            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = cache.ByUser
                    .SelectMany(x => x.Value)
                    .Where(x => Math.Abs((x.Value.Date - DateTimeOffset.Now).TotalSeconds) < 35)
                    .ToImmutableArray();

                if (notifications.Any())
                {
                    var tasks = notifications.AsParallel().Select(async x =>
                    {
                        cache.ByUser[x.Value.ChatId].Remove(x);
                        await remover.Remove(x.Value);
                        await bot.SendTextMessageAsync(x.Value.ChatId, x.ToString());
                    });

                    await Task.WhenAll(tasks);
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        private void StartBot(IServiceScope scope, ITelegramBotClient bot)
        {
            var notifyService = scope.ServiceProvider.GetService<NotifyService>();

            bot!.OnMessage += notifyService!.OnMessage;
            bot.OnCallbackQuery += notifyService!.OnCallbackQuery;
            bot.StartReceiving();
        }
    }
}
