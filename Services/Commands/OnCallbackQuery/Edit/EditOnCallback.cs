using Common;
using Common.CallbackModels;
using Services.Cache;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery.Edit
{
    public class EditOnCallback : ICallbackCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public EditOnCallback(NotifyCacheService cache, TelegramBotClient bot)
        {
            _bot = bot;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var chatId = e.CallbackQuery.Message.Chat.Id;
            _cache.ByUser.TryGetValue(chatId, out var models);

            var callbackData = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
            models!.TryGetValue(callbackData.NotifyId, out var model);

            var markup = new InlineKeyboardMarkup(new[]{
                new InlineKeyboardButton
                {
                    Text = "Название",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.Edit.NotifyName, model!.NotifyId),
                },
                new InlineKeyboardButton
                {
                    Text = "Дату",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.Edit.NotifyDate, model!.NotifyId),
                },
                new InlineKeyboardButton
                {
                    Text = "Частоту",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.Edit.NotifyFrequency, model!.NotifyId),
                }
            });

            await _bot.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);
            await _bot.SendTextMessageAsync(chatId, "Что меняем?", replyMarkup: markup);
        }
    }
}
