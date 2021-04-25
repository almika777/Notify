using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Commands;
using Notify.Common;
using Notify.Helpers;
using Notify.IO;
using Notify.Services;
using Notify.Workers;
using Telegram.Bot;

namespace Notify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<Configuration>(hostContext.Configuration.GetSection(nameof(Configuration)));
                    services.AddScoped<INotifyIOHandler, NotifyFileHandler>();
                    services.AddScoped<OnMessageCommandRepository>();
                    services.AddScoped<NotifyService>();
                    services.AddScoped<NotifyModifier>();

                    services.AddSingleton(x => new TelegramBotClient(Resource.TelegramToken));
                    services.AddSingleton<NotifyCacheService>();

                    services.AddHostedService<NotifyWorker>();
                });
    }
}
