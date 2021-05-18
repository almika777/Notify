using Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify;
using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NotifyTests.IoTests
{
    public class IoBaseTests : BaseTest
    {
        public IoBaseTests()
        {
            SetProvider();
        }

        protected IServiceProvider Services;

        protected IServiceProvider SetProvider()
        {
            var host = Program.CreateHostBuilder(null);
            host.ConfigureServices(collection => Program.AddDbContext(collection, "../../../../../NotifiesTestsDB.db"));
            Services = host.Build().Services;

            Services.GetService<NotifyDbContext>().Database.EnsureCreated();
            return Services;
        }

        [TearDown]
        protected async Task ClearDb()
        {
            var context = Services.GetService<NotifyDbContext>();
            await context.Database.EnsureDeletedAsync();
        }
    }
}
