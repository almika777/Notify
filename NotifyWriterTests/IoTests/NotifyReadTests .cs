using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Context;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.IoServices;
using System.Threading.Tasks;
using Common.Models;

namespace NotifyTests.IoTests
{
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
        public async Task ReadNotifyTest()
        {
            var model = GetDefaultModel();
            var entry = _mapper.Map<Context.Entities.Notify>(model);

            await _context.Notifies.AddAsync(entry);
            await _context.SaveChangesAsync();

            var entity = await _reader.Read(model.UserId, model.NotifyId);

            Assert.AreEqual(entity, model);
        }

        [Test]
        public async Task ReadAllNotifyTest()
        {
            var model = GetDefaultModel();
            var model2 = GetDefaultModel();
            model2.NotifyId = Guid.NewGuid();

            var entry = _mapper.Map<Context.Entities.Notify>(model);
            var entry2 = _mapper.Map<Context.Entities.Notify>(model2);

            await _context.Notifies.AddRangeAsync(new List<Context.Entities.Notify> { entry, entry2 });
            await _context.SaveChangesAsync();

            var entities = await _reader.ReadAll(model.UserId);

            Assert.AreEqual(entities.Length, 2);
            Assert.AreEqual(entities.First(), model);
            Assert.AreEqual(entities.Last(), model2);
        }
    }
}
