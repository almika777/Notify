using Common;
using CommonTests;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Services.Services.IoServices;
using System.IO;
using Services.Services.IoServices.FileServices;

namespace NotifyTests
{
    public class NotifyFileWriterTests : TestBase
    {
        private INotifyWriter _writer;
        private Configuration _config;

        [SetUp]
        public void Setup()
        {
            _config = GlobalConfig.GetDefault().Value;
            _writer = new NotifyFileWriter(GlobalConfig.GetDefault(), new NullLogger<NotifyFileWriter>());
        }

        [Test]
        public void WriteModelTest()
        {
            _writer.Write(GlobalModel.GetModel());

            Assert.IsTrue(Directory.Exists(_config.CacheFolder));
            Finish(_config);
        }
    }
}