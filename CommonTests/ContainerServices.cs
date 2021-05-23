using System;
using System.Threading.Tasks;
using Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify;
using NUnit.Framework;

namespace CommonTests
{
    public class ContainerServices : BaseTest
    {
        public ContainerServices()
        {
            SetProvider();
        }

        protected IServiceProvider Services;

        [SetUp]
        protected void SetProvider()
        {
            Services = Program.CreateHostBuilder("appsettings.json").Build().Services;

            var context = Services.GetRequiredService<NotifyDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [TearDown]
        protected async Task ClearDb()
        {
            var context = Services.GetService<NotifyDbContext>();
            await context.Database.EnsureDeletedAsync();
        }
    }
}
