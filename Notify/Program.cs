using System.Diagnostics;
using System.IO;
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
        private static Configuration _config = null!;

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(new CompactJsonFormatter(), _config.LogPath, shared: true)
                .WriteTo.Console()
                .CreateLogger();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Configure(hostContext, services);
                    AddDbContext(hostContext, services);
                    AddSingletonServices(services);
                    AddScopedServices(services);

                    services.AddHostedService<NotifyWorker>();
                })
                .UseSerilog();

        public static void AddDbContext(HostBuilderContext hostContext, IServiceCollection services)
        {
            var dbPath = hostContext.Configuration.GetSection("Configuration").GetValue<string>("DbPath");
            var connectionString = $@"Data Source ={dbPath};";
            services.AddDbContext<NotifyDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
        }

        private static void Configure(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.Configure<Configuration>(hostContext.Configuration.GetSection(nameof(Configuration)));
            _config = hostContext.Configuration.GetSection(nameof(Configuration)).Get<Configuration>();
        }

        private static void AddSingletonServices(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Services.Mapper());
            });

            services.AddSingleton(mapperConfig.CreateMapper());
            services.AddSingleton(x => new TelegramBotClient(_config.TelegramToken));
            services.AddSingleton<UserCacheService>();
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
