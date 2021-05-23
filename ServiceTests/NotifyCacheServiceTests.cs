using CommonTests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.Cache;
using Services.IoServices;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceTests
{
    public class NotifyCacheServiceTests : ContainerServices
    {
        private NotifyCacheService _cache;
        private INotifyWriter _writer;

        [SetUp]
        public void Setup()
        {
            _cache = Services.GetService<NotifyCacheService>();
            _writer = Services.GetService<INotifyWriter>();
        }

        [Test]
        public async Task InitializeTests()
        {
            var userId = 1;
            var models = GetDefaultManyModels(userId).ToArray();

            await _writer.Write(models);
            await _cache.Initialize();

            Assert.IsTrue(_cache.IsInitialized);
            Assert.IsNotEmpty(_cache.ByUser);
            Assert.IsTrue(_cache.ByUser.TryGetValue(userId, out var userNotifies));
            Assert.IsNotEmpty(userNotifies);
            Assert.AreEqual(models.Length, userNotifies.Count);
        }

        [Test]
        public void TryRemoveFromCurrentTests()
        {
            var model = GetDefaultModel();

            _cache.InProgressNotifications.TryAdd(model.ChatId, model);
            var isDeleted = _cache.TryRemoveFromCurrent(model);

            Assert.IsTrue(isDeleted);
            Assert.IsEmpty(_cache.InProgressNotifications);
        }

        [Test]
        public void TryAddToMemoryOneTests()
        {
            var model = GetDefaultModel();

            _cache.TryAddToByUser(model);

            Assert.IsNotEmpty(_cache.ByUser);
        }

        [Test]
        public void TryAddToMemoryManyTests()
        {
            var userId = 1;
            var models = GetDefaultManyModels(userId).ToArray();

            foreach (var notifyModel in models)
            {
                _cache.TryAddToByUser(notifyModel);
            }

            Assert.IsNotEmpty(_cache.ByUser);
            Assert.AreEqual(_cache.ByUser.Count, 1);
            Assert.IsTrue(_cache.ByUser.TryGetValue(userId, out var userNotifies));
            Assert.IsNotEmpty(userNotifies);
            Assert.AreEqual(userNotifies.Count, models.Length);
        }
    }
}