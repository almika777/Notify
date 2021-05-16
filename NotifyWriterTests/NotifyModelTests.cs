using CommonTests;
using NUnit.Framework;
using System;
using Common;

namespace NotifyTests
{
    public class NotifyModelTests : TestBase
    {
        private Configuration _config;
        private Common.Models.NotifyModel _model;

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
            Assert.AreEqual(model, string.Join(CommonResource.Separator, _model.UserId, _model.NotifyId, _model.Name, _model.Date.ToString("g")));
            Finish(_config);
        }

        [Test]
        public void FromStringTest()
        {
            var stringModel = _model.ToString();
            var model = Common.Models.NotifyModel.FromString(stringModel);

            var properties = stringModel.Split(CommonResource.Separator);

            Assert.AreEqual(model.UserId, long.Parse(properties[0]));
            Assert.AreEqual(model.NotifyId, Guid.Parse(properties[1]));
            Assert.AreEqual(model.Name, properties[2]);
            Assert.AreEqual(model.Date, DateTimeOffset.Parse(properties[3]));

            Finish(_config);
        }
    }
}