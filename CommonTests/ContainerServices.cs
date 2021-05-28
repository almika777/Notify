using Context;
using Microsoft.Extensions.DependencyInjection;
using Notify;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CommonTests
{
    public class ContainerServices : BaseTest
    {
        protected IServiceProvider Services;

        [SetUp]
        protected void SetProvider()
        {
            Directory.SetCurrentDirectory(@"C:\Users\xkang\source\repos\Notify\CommonTests");
            Services = Program.CreateHostBuilder(null).Build().Services;

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
