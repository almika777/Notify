using AutoMapper;
using Common.Enum;
using Common.Models;
using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.IoServices;
using System;
using System.Threading.Tasks;

namespace NotifyTests.IoTests
{
    public class NotifyWriteTests : IoBaseTests
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
        public async Task WriteNotifyTest()
        {
            var model = new NotifyModel
            {
                Date = DateTimeOffset.Now,
                Name = "test",
                UserId = 1,
                Frequency = FrequencyType.Minute,
                ChatUserModel = new ChatUserModel { ChatId = 1 },
                FrequencyTime = TimeSpan.FromMinutes(1),
                NotifyId = Guid.NewGuid()
            };

            await _writer.Write(model);

            var notifies = await _context.Notifies.ToArrayAsync();

            Assert.IsNotEmpty(notifies);
            Assert.AreEqual(notifies.Length, 1);
            Assert.AreEqual(model, _mapper.Map<NotifyModel>(notifies[0]));
        }
    }
}
