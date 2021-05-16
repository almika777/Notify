using Common;
using Common.CallbackModels;
using Common.Enum;
using Services.Cache;
using System;
using System.Threading.Tasks;
using Common.Models;
using Services.Helpers.NotifyStepHandlers;
using Telegram.Bot;
using Telegram.Bot.Args;

// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery.Edit
{
    public class EditNotifyOnCallback : ICallbackCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyStepHandlers _stepHandlers;
        private readonly NotifyCacheService _cache;

        public EditNotifyOnCallback(TelegramBotClient bot, NotifyStepHandlers stepHandlers, NotifyCacheService cache)
        {
            _bot = bot;
            _stepHandlers = stepHandlers;
            _cache = cache;
        }

        public Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var callbackData = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
            var chatId = e.CallbackQuery.Message.Chat.Id;
            var fieldType = GetFieldType(callbackData.Command);
            var answer = GetAnswer(fieldType);

            _cache.EditCache.TryAdd(chatId, (callbackData.NotifyId, fieldType));
            _bot.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);

            switch (fieldType)
            {
                case EditField.Name:
                case EditField.Date: return _bot.SendTextMessageAsync(chatId, answer);
                case EditField.Frequency: return _stepHandlers.FrequencyStep.Execute(chatId, new NotifyModel { NextStep = NotifyStep.Frequency });
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private string GetAnswer(EditField fieldType) => fieldType switch
        {
            EditField.Name => "Новое имя события",
            EditField.Date => "Новая дата",
            EditField.Frequency => "Выберите новый период",
            _ => throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null)
        };

        private EditField GetFieldType(string callbackDataCommand)
        {
            if (callbackDataCommand == BotCommands.OnCallback.Edit.NotifyName)
                return EditField.Name;

            if (callbackDataCommand == BotCommands.OnCallback.Edit.NotifyDate)
                return EditField.Date;

            return EditField.Frequency;
        }
    }
}
