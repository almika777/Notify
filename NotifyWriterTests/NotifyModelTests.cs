using System;
using CommonTests;
using Notify.Common;
using NUnit.Framework;

namespace NotifyTests
{
    public class NotifyModelTests : TestBase
    {
        private Configuration _config;
        private NotifyModel _model;

        [SetUp]
        public void Setup()
        {
            _config = GlobalConfig.GetDefault().Value;
            _model = GlobalModel.GetModel();
        }

        [Test]
        public void ToStringTest()
        {
            var model = _model.ToString();
            Assert.AreEqual(model, string.Join(CommonResource.Separator, _model.ChatId, _model.NotifyId, _model.Name, _model.Date.ToString("g")));
            Finish(_config);
        }

        [Test]
        public void FromStringTest()
        {
            var stringModel = _model.ToString();
            var model = NotifyModel.FromString(stringModel);

            var properties = stringModel.Split(CommonResource.Separator);

            Assert.AreEqual(model.ChatId, long.Parse(properties[0]));
            Assert.AreEqual(model.NotifyId, Guid.Parse(properties[1]));
            Assert.AreEqual(model.Name, properties[2]);
            Assert.AreEqual(model.Date, DateTimeOffset.Parse(properties[3]));

            Finish(_config);
        }
    }
}