using AutoMapper;
using Common;
using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Workers;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Services;
using Services.Cache;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using Services.IoServices;
using Services.IoServices.SQLiteServices;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Notify
{
    public class Program
    {
        private static IConfigurationSection _configurationSection = null!;

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(new CompactJsonFormatter(), @"../../logs/log.json", shared: true)
                .WriteTo.Console()
                .CreateLogger();

             await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    AddDbContext(services, "../../NotifiesDB.db");
                    Configure(hostContext, services);
                    AddSingletonServices(services);
                    AddScopedServices(services);

                    services.AddHostedService<NotifyWorker>();
                }).UseSerilog();

        public static void AddDbContext(IServiceCollection services, string path)
        {
            services.AddDbContext<NotifyDbContext>(options =>
            {
                options.UseSqlite($@"Data Source={path};");
            });
        }

        private static void Configure(HostBuilderContext hostContext, IServiceCollection services)
        {
            _configurationSection = hostContext.Configuration.GetSection(nameof(Configuration));
            services.Configure<Configuration>(_configurationSection);
        }

        private static void AddSingletonServices(IServiceCollection services)
        {
            var config = _configurationSection.Get<Configuration>();
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Services.Mapper());
            });

            services.AddSingleton(mapperConfig.CreateMapper());
            services.AddSingleton(x => new TelegramBotClient(config.TelegramToken));
            services.AddSingleton<NotifyCacheService>();
        }

        private static void AddScopedServices(IServiceCollection services)
        {
            services.AddScoped<INotifyWriter, NotifyWriter>();
            services.AddScoped<INotifyReader, NotifyReader>();
            services.AddScoped<INotifyRemover, NotifyRemover>();
            services.AddScoped<INotifyEditor, NotifyEditor>();

            services.AddScoped<NotifyStepHandlers>();
            services.AddScoped<OnMessageCommandRepository>();
            services.AddScoped<OnCallbackRepository>();

            services.AddScoped<NotifyModifier>();
            services.AddScoped<NotifyService>();


        }


    }
}
