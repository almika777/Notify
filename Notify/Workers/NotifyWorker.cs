using Common.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Cache;
using Services.IoServices;
using System;
using System.Collections.Generic;
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
                Console.WriteLine(e);
                throw;
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
