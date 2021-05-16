using Common.CallbackModels;
using Common.Enum;
using Common.Models;
using Services.Cache;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery
{
    public class SetFrequencyOnCallback : ICallbackCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyModifier _modifier;
        private readonly NotifyStepHandlers _stepHandlers;
        private readonly NotifyCacheService _cache;

        public SetFrequencyOnCallback(NotifyCacheService cache, TelegramBotClient bot, NotifyModifier modifier, NotifyStepHandlers stepHandlers)
        {
            _bot = bot;
            _modifier = modifier;
            _stepHandlers = stepHandlers;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var chatId = e.CallbackQuery.Message.Chat.Id;
            var model = GetUpdatedModel(chatId);

            if (model!.NextStep != NotifyStep.Frequency)
            {
                await _bot.SendTextMessageAsync(chatId, "");
                return;
            }

            var frequency = CallbackFrequency.FromCallback(e.CallbackQuery.Data);
            model.Frequency = frequency;
            model.FrequencyTime = GetFrequencyTime(frequency);

            if (frequency == FrequencyType.Custom)
            {
                await _bot.SendTextMessageAsync(chatId, $@"Укажите как часто нужно воспроизводить в формате{Environment.NewLine}01.01.0001 00:00");
            }
            else
            {
                _modifier.CreateOrUpdate(model, $"{(int)model.Frequency}");
                await _stepHandlers.ReadyStep.Execute(chatId, model);
            }
        }

        private NotifyModel GetUpdatedModel(long chatId)
        {
            NotifyModel model = null!;

            if (_cache.InProgressNotifications.TryGetValue(chatId, out var newModel))
            {
                model = newModel!;
                return model;
            }

            if (!_cache.EditCache.TryGetValue(chatId, out var value)) return model;
            if (!_cache.ByUser.TryGetValue(chatId, out var models)) return model;

            models.TryGetValue(value.NotifyId, out var editedModel);
            model = editedModel!;
            model.NextStep = NotifyStep.Frequency;
            return model;
        }

        private TimeSpan GetFrequencyTime(FrequencyType type) => type switch
        {
            FrequencyType.Day => TimeSpan.FromDays(1),
            FrequencyType.Minute => TimeSpan.FromMinutes(1),
            FrequencyType.Hour => TimeSpan.FromHours(1),
            FrequencyType.Week => TimeSpan.FromDays(7),
            FrequencyType.Once => TimeSpan.MaxValue,
            FrequencyType.Custom => TimeSpan.MaxValue,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
