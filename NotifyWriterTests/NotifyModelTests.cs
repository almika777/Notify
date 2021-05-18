using Common.Enum;
using Common.Models;
using NUnit.Framework;

namespace NotifyTests
{

    public class NotifyModelTests
    {
        private NotifyModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new NotifyModel();
        }

        [Test]
        public void GetNextStepMessageTest()
        {
            _model.NextStep = NotifyStep.Name;
            Assert.AreEqual(_model.GetNextStepMessage(), "Введите название");

            _model.NextStep = NotifyStep.Date;
            Assert.AreEqual(_model.GetNextStepMessage(), "Введите дату и время в формате 01.01.2021 00:00");

            _model.NextStep = NotifyStep.Frequency;
            Assert.AreEqual(_model.GetNextStepMessage(), "Выберите периодичность");

            _model.NextStep = NotifyStep.Ready;
            Assert.AreEqual(_model.GetNextStepMessage(), "Готово");
        }
    }
}
