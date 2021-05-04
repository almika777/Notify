using Common.Common;
using Common.Common.Enum;
using System;
using Telegram.Bot.Args;

namespace Services.Helpers
{
    public class NotifyModifier
    {
        public NotifyModel CreateOrUpdate(NotifyModel? model, MessageEventArgs e)
        {
            var newModel = model ?? new NotifyModel { ChatId = e.Message.Chat.Id, NotifyId = Guid.NewGuid(), Name = e.Message.Text };
            return Update(newModel, e);
        }

        public string GetNextStepMessage(NotifyModel model) => model.NextStep switch
        {
            NotifyStep.Date => "Введите дату и время в формате 01.01.2021 00:00",
            NotifyStep.Frequency => "Выберите периодичность",
            NotifyStep.Ready => "Готово",
            _ => throw new ArgumentOutOfRangeException()
        };

        private NotifyModel Update(NotifyModel model, MessageEventArgs e)
        {
            switch (model.NextStep)
            {
                case NotifyStep.Name:
                    model.Name = e.Message.Text; break;
                case NotifyStep.Date:
                    model.Date = DateTimeOffset.TryParse(e.Message.Text, out var date) ? date : throw new FormatException("Неверный формат даты");
                    break;
                case NotifyStep.Frequency:
                    model.Frequency = (FrequencyType)int.Parse(e.Message.Text);
                    break;
            }

            return UpdateState(model);
        }

        private NotifyModel UpdateState(NotifyModel model)
        {
            model.NextStep = model.NextStep switch
            {
                NotifyStep.Name => NotifyStep.Date,
                NotifyStep.Date => NotifyStep.Frequency,
                NotifyStep.Frequency => NotifyStep.Ready,
                _ => model.NextStep
            };
            return model;
        }
    }
}
