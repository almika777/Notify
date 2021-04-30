using Common.Common;
using CommonTests;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Threading.Tasks;
using Services.Services.IoServices;
using Services.Services.IoServices.File;

namespace NotifyTests
{
    public class NotifyFileReaderTests : TestBase
    {
        private INotifyReader _reader;
        private INotifyWriter _writer;
        private Configuration _config;
        private NotifyModel _model;

        [SetUp]
        public void Setup()
        {
            _config = GlobalConfig.GetDefault().Value;
            _model = GlobalModel.GetModel();
            _writer = new NotifyFileWriterService(GlobalConfig.GetDefault(), new NullLogger<NotifyFileWriterService>());
            _reader = new NotifyFileReaderService(GlobalConfig.GetDefault(), new NullLogger<NotifyFileReaderService>());
        }

        [Test]
        public async Task WriteModelTest()
        {
            await _writer.Write(_model);
            var model = await _reader.Read(_model.ChatId, _model.NotifyId);

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ChatId, _model.ChatId);
            Assert.AreEqual(model.NotifyId, _model.NotifyId);
            Assert.AreEqual(model.Name, _model.Name);
            Assert.AreEqual(model.Date, _model.Date);
            Finish(_config);
        }
    }
}