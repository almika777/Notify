using Common.Common;
using Common.Common.Enum;
using System;

namespace Services.Helpers
{
    public class NotifyModifier
    {
        public NotifyModel CreateOrUpdate(NotifyModel? model, long chatId, string data)
        {
            var newModel = model ?? new NotifyModel { ChatId = chatId, NotifyId = Guid.NewGuid(), Name = data };
            return Update(newModel, data);
        }

        public NotifyModel CreateOrUpdate(NotifyModel? model, string data) => Update(model!, data);

        private NotifyModel Update(NotifyModel model, string data)
        {
            switch (model.NextStep)
            {
                case NotifyStep.Name:
                    model.Name = data; break;
                case NotifyStep.Date:
                    model.Date = DateTimeOffset.TryParse(data, out var date) ? date : throw new FormatException("Неверный формат даты");
                    break;
                case NotifyStep.Frequency:
                    model.Frequency = (FrequencyType)int.Parse(data);
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
