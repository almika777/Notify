using Notify.Common;
using System;
using Telegram.Bot.Args;

namespace Notify.Helpers
{
    public class NotifyModifier
    {
        public NotifyModel Create(MessageEventArgs e)
        {
            var model = new NotifyModel {ChatId = e.Message.Chat.Id, NotifyId = Guid.NewGuid(), Name = e.Message.Text};
            return model;
        }

        public NotifyModel Update(NotifyModel model, MessageEventArgs e)
        {
            switch (model.CurrentState)
            {
                case NotifyState.Name:
                    model.Name = e.Message.Text; break;
                case NotifyState.Date:
                    model.Date = DateTimeOffset.TryParse(e.Message.Text, out var date) ? date : throw new FormatException("Неверный формат даты");
                    break;
            }

            return UpdateState(model);
        }

        public string GetNextStepMessage(NotifyModel model) => model.CurrentState switch
        {
            NotifyState.Name => "Введите дату и время в формате 01.01.2021 00:00",
            NotifyState.Date => "Готово",
            _ => throw new ArgumentOutOfRangeException()
        };

        private NotifyModel UpdateState(NotifyModel model)
        {
            model.CurrentState = model.CurrentState switch
            {
                NotifyState.Name => NotifyState.Date,
                _ => model.CurrentState
            };
            return model;
        }
    }
}
