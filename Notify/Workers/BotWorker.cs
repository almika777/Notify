using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Services;
using Telegram.Bot;

namespace Notify.Workers
{
    public class BotWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BotWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var notifyService = scope.ServiceProvider.GetService<NotifyService>();
            var bot = scope.ServiceProvider.GetService<TelegramBotClient>();

            bot!.OnMessage += notifyService!.OnMessage;
            bot.OnCallbackQuery += notifyService!.OnOnCallbackQuery;

            bot.StartReceiving();
        }
    }
}
