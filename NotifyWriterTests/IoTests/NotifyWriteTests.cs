using AutoMapper;
using Common.Models;
using CommonTests;
using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.IoServices;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace NotifyTests.IoTests
{
    public class NotifyWriteTests : ContainerServices
    {
        private NotifyDbContext _context;
        private INotifyWriter _writer;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _context = Services.GetRequiredService<NotifyDbContext>();
            _writer = Services.GetRequiredService<INotifyWriter>();
            _mapper = Services.GetRequiredService<IMapper>();
        }

        [Test]
        public async Task WriteOneNotifyTest()
        {
            var model = GetDefaultModel();

            await _writer.Write(model);

            var notifies = await _context.Notifies.ToArrayAsync();

            Assert.IsNotEmpty(notifies);
            Assert.AreEqual(notifies.Length, 1);
            Assert.AreEqual(model, _mapper.Map<NotifyModel>(notifies[0]));
        }

        [Test]
        public async Task WriteManyNotifiesTest()
        {
            var models = GetDefaultManyModels().ToImmutableArray();

            await _writer.Write(models);

            var notifies = await _context.Notifies.ToArrayAsync();

            Assert.IsNotEmpty(notifies);
            Assert.AreEqual(notifies.Length, models.Length);

            for (int i = 0; i < models.Length; i++)
            {
                Assert.AreEqual(models[i], _mapper.Map<NotifyModel>(notifies[i]));
            }
        }
    }
}
