using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notify;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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
            var host = Program.CreateHostBuilder("Data Source=../../../../../NotifiesDbTests.db");
            Services = host.Build().Services;
            var context = Services.GetRequiredService<NotifyDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

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
