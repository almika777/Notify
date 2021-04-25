using Notify.Common;
using System;
using Telegram.Bot.Args;

namespace Notify.Helpers
{
    public class NotifyModifier
    {
        public NotifyModel Create(MessageEventArgs e)
        {
            var newModel = new NotifyModel {ChatId = e.Message.Chat.Id, NotifyId = Guid.NewGuid()};
            return Update(newModel, e);
        }

        public NotifyModel Update(NotifyModel model, MessageEventArgs e)
        {
            switch (model.State)
            {
                case NotifyState.Name:
                    model.Name = e.Message.Text; break;
                case NotifyState.Date:
                    model.Date = DateTimeOffset.TryParse(e.Message.Text, out var date) ? date : throw new FormatException("Неверный формат даты");
                    break;
            }

            model.UpdateState();
            return model;
        }

        public string GetNextStepMessage(NotifyModel model) => model.State switch
        {
            NotifyState.Name => "Введите повод",
            NotifyState.Date => "Введите дату и время в формате 01.01.2021 00:00",
            NotifyState.Ready => "Готово",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
