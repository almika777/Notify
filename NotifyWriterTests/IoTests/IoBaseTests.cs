using Context;
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

        [SetUp]
        protected void SetProvider()
        {
            Services = Program
                .CreateHostBuilder("Data Source=../../../../../NotifiesDbTests.db")
                .Build()
                .Services;

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
