using Common;
using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Workers;
using Services;
using Services.Cache;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using Services.IoServices;
using Services.IoServices.FileServices;
using Services.IoServices.SQLiteServices;
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
                    Configure(hostContext, services);
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
            services.AddScoped<INotifyWriter, NotifyWriter>();
            services.AddScoped<INotifyReader, NotifyReader>();
            services.AddScoped<INotifyRemover, NotifyRemover>();
            services.AddScoped<INotifyEditor, NotifyFileEditor>();

            services.AddScoped<NotifyStepHandlers>();
            services.AddScoped<OnMessageCommandRepository>();
            services.AddScoped<OnCallbackCommandRepository>();

            services.AddScoped<NotifyModifier>();
            services.AddScoped<NotifyService>();

            services.AddDbContext<NotifyDbContext>(options => options.UseSqlite("Data Source=../../NotifiesDB.db;"));
        }
    }
}
