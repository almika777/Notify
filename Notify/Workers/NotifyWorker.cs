using Common.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using Services.Cache;
using Services.IoServices;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Notify.Workers
{
    public class NotifyWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotifyWorker> _logger;

        public NotifyWorker(IServiceProvider serviceProvider, ILogger<NotifyWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var bot = scope.ServiceProvider.GetService<TelegramBotClient>()!;
            var remover = scope.ServiceProvider.GetService<INotifyRemover>()!;
            var cache = scope.ServiceProvider.GetRequiredService<NotifyCacheService>();

            await cache.Initialize();
            StartBot(scope);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var notifications = cache.ByUser
                        .SelectMany(x => x.Value)
                        .Where(x =>
                            (x.Value.Date - DateTimeOffset.Now).TotalSeconds < 30)
                        .ToImmutableArray();

                    if (notifications.Any())
                    {
                        var tasks = notifications.AsParallel().Select(async x =>
                        {
                            if (x.Value.Frequency != FrequencyType.Once)
                            {
                                await bot.SendTextMessageAsync(x.Value.UserId, x.Value.ToTelegramChat(), ParseMode.Html);
                                x.Value.Date += x.Value.FrequencyTime;
                                return;
                            }

                            cache.ByUser[x.Value.UserId].Remove(x);
                            await remover.Remove(x.Value);
                        });

                        await Task.WhenAll(tasks);
                    }


                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Упс");
            }
        }

        private void StartBot(IServiceScope serviceScope)
        {
            var bot = serviceScope.ServiceProvider.GetService<TelegramBotClient>();
            var notifyService = serviceScope.ServiceProvider.GetService<NotifyService>();

            bot!.OnMessage += notifyService!.OnMessage;
            bot.OnCallbackQuery += notifyService!.OnCallbackQuery;
            bot.StartReceiving();
        }
    }
}
