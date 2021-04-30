using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Notify.Common;
using Notify.IO;
using Notify.IO.File;
using NUnit.Framework;
using TestsCommon;

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