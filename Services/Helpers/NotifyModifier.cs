﻿using Common;
using System;
using System.Globalization;
using Common.Enum;
using Common.Models;

namespace Services.Helpers
{
    public class NotifyModifier
    {
        public NotifyModel CreateOrUpdate(NotifyModel model, string data)
        {
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
