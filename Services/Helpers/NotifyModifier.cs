using Common;
using Common.Enum;
using Common.Models;
using System;
using System.Globalization;
using Telegram.Bot.Args;

namespace Services.Helpers
{
    public class NotifyModifier
    {
        public void EditNotify(MessageEventArgs e, NotifyModel model, EditField field)
        {
            switch (field)
            {
                case EditField.Name: model!.Name = e.Message.Text.Trim(); break;
                case EditField.Date:
                    {
                        DateTimeOffset.TryParse(e.Message.Text.Trim(), out var date);
                        model!.Date = date;
                        break;
                    }
                case EditField.Frequency:
                    {
                        CreateOrUpdate(model!, e.Message.Text);
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(field), field, null);
            }
        }

        public NotifyModel CreateOrUpdate(NotifyModel? model, string data)
        {
            if (model == null) return new NotifyModel
            {
                NextStep = NotifyStep.Date,
                NotifyId = Guid.NewGuid(),
                ChatUserModel = new ChatUserModel()
            };

            switch (model.NextStep)
            {
                case NotifyStep.Name:
                    model.Name = data; break;
                case NotifyStep.Date:
                    model.Date = DateTimeOffset
                        .TryParseExact(data, CommonResource.DateFormats, new DateTimeFormatInfo(), DateTimeStyles.None, out var date)
                        ? date
                        : throw new FormatException("Неверный формат даты");
                    break;
                case NotifyStep.Frequency:
                    {
                        if (model.Frequency == FrequencyType.Custom)
                        {
                            var frequencyTime = TimeSpan.TryParse(data, out var time)
                                ? time
                                : throw new FormatException("Неверный формат времени");
                            model.FrequencyTime = frequencyTime;
                            break;
                        }
                        model.Frequency = (FrequencyType)int.Parse(data);
                        break;
                    }

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
