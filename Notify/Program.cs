using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Commands;
using Notify.IO;
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
                    services.AddScoped<INotifyIOHandler, NotifyFileHandler>();
                    services.AddScoped<CommandRepository>();
                    services.AddScoped<NotifyService>();

                    services.AddSingleton(x => new TelegramBotClient(Resource.TelegramToken));
                    services.AddSingleton<NotifyCache>();


                    services.AddHostedService<Startup>();
                    services.AddHostedService<NotifyWorker>();
                });
    }
}
