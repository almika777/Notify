using AutoMapper;
using Context;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.IoServices;
using System.Linq;
using System.Threading.Tasks;

namespace NotifyTests.IoTests
{
    [TestFixture]
    public class NotifyReadTests : IoBaseTests
    {
        private NotifyDbContext _context;
        private INotifyReader _reader;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _context = Services.GetRequiredService<NotifyDbContext>();
            _reader = Services.GetRequiredService<INotifyReader>();
            _mapper = Services.GetRequiredService<IMapper>();
        }

        [Test]
        public async Task ReadNotifyWithOneUserTest()
        {
            var model = GetDefaultModel();
            var entry = _mapper.Map<Context.Entities.Notify>(model);

            await _context.Notifies.AddAsync(entry);
            await _context.SaveChangesAsync();

            var entity = await _reader.Read(entry.UserId, entry.NotifyId);

            Assert.AreEqual(entity, model);
        }

        [Test]
        public async Task ReadNotifyWithManyUsersTest()
        {
            var model = GetDefaultModel();
            var model2 = GetDefaultModel(2);

            var entry = _mapper.Map<Context.Entities.Notify>(model);
            var entry2 = _mapper.Map<Context.Entities.Notify>(model2);

            await _context.Notifies.AddAsync(entry);
            await _context.Notifies.AddAsync(entry2);
            await _context.SaveChangesAsync();

            var entity = await _reader.Read(entry.UserId, entry.NotifyId);

            Assert.AreEqual(entity, model);
        }

        [Test]
        public async Task ReadAllNotifyWithOneUserTest()
        {
            var models = GetDefaultManyModels().ToArray();
            var entries = _mapper.Map<Context.Entities.Notify[]>(models);

            await _context.AddRangeAsync(entries);
            await _context.SaveChangesAsync();

            var entities = await _reader.ReadAll(entries.First().UserId);

            Assert.AreEqual(entities.Length, models.Length);
            Assert.AreEqual(entities.First(), models.First());
            Assert.AreEqual(entities.Last(), models.Last());
        }

        [Test]
        public async Task ReadAllNotifyWithManyUserTest()
        {

            var models = GetDefaultManyModels().ToArray();
            var models2 = GetDefaultManyModels(2).ToArray();

            var entries = _mapper.Map<Context.Entities.Notify[]>(models);
            var entries2 = _mapper.Map<Context.Entities.Notify[]>(models2);

            await _context.AddRangeAsync(entries);
            await _context.AddRangeAsync(entries2);
            await _context.SaveChangesAsync();

            var entities1 = await _reader.ReadAll(entries[0].UserId);
            var entities2 = await _reader.ReadAll(entries2[0].UserId);

            Assert.AreEqual(entities1.Length, models.Length);
            Assert.AreEqual(entities2.Length, models2.Length);
        }
    }
}
