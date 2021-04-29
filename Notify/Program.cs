using Microsoft.Extensions.Configuration;
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
        private static IConfigurationSection _configurationSection;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Configure(hostContext,services);
                    AddSingletonServices(services);
                    AddScopedServices(services);

                    services.AddHostedService<NotifyWorker>();
                });

        private static void Configure(HostBuilderContext hostContext, IServiceCollection services)
        {
            _configurationSection = hostContext.Configuration.GetSection(nameof(Configuration));
            services.Configure<Configuration>(_configurationSection);

        }

        private static void AddSingletonServices(IServiceCollection services)
        {
            var config = _configurationSection.Get<Configuration>();

            services.AddSingleton(x => new TelegramBotClient(config.TelegramToken));
            services.AddSingleton<NotifyCacheService>();
        }

        private static void AddScopedServices(IServiceCollection services)
        {
            services.AddScoped<INotifyWriter, NotifyFileWriter>();
            services.AddScoped<OnMessageCommandRepository>();
            services.AddScoped<NotifyModifier>();
            services.AddScoped<NotifyService>();
        }
    }
}
