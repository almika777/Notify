using Common.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Workers;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using Services.Helpers;
using Services.Services;
using Services.Services.IoServices;
using Services.Services.IoServices.FileServices;
using Telegram.Bot;

namespace Notify
{
    public class Program
    {
        private static IConfigurationSection _configurationSection = null!;

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
            services.AddScoped<INotifyReader, NotifyFileReader>();
            services.AddScoped<INotifyRemover, NotifyFileRemove>();
            services.AddScoped<INotifyEditor, NotifyFileEditor>();

            services.AddScoped<IoRepository>();
            services.AddScoped<OnMessageCommandRepository>();
            services.AddScoped<OnCallbackCommandRepository>();

            services.AddScoped<NotifyModifier>();
            services.AddScoped<NotifyService>();
        }
    }
}
