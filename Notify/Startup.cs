using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify
{
    public class Startup : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public Startup(IServiceProvider serviceProvider)
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
