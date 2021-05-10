using Common;
using Services.Services;
using System;
using System.Threading.Tasks;
using Common.CallbackModels;
using Common.Enum;
using Telegram.Bot;
using Telegram.Bot.Args;

// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery.Edit
{
    public class EditNotifyOnCallback : ICallbackCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public EditNotifyOnCallback(NotifyCacheService cache, TelegramBotClient bot)
        {
            _bot = bot;
            _cache = cache;
        }

        public Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var callbackData = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);

            var fieldType = GetFieldType(callbackData.Command);
            var answer = GetAnswer(fieldType);
            _cache.EditCache.TryAdd(e.CallbackQuery.Message.Chat.Id, (callbackData.NotifyId, fieldType));
            _bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId);

            return _bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, answer);
        }

        private string GetAnswer(EditField fieldType)
        {
            return fieldType switch
            {
                EditField.Name => "Новое имя события",
                EditField.Date => "Новая дата",
                _ => throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null)
            };
        }

        private EditField GetFieldType(string callbackDataCommand)
        {
            return callbackDataCommand == BotCommands.OnCallback.EditNotifyName 
                ? EditField.Name 
                : EditField.Date;
        }
    }
}
