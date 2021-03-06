using Common;
using Common.CallbackModels;
using System;
using System.Threading.Tasks;
using Services.Cache;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Services.Commands.OnCallbackQuery
{
    public class ShowOnCallback : ICallbackCommand
    {
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;

        public ShowOnCallback(NotifyCacheService cache, TelegramBotClient bot)
        {
            _cache = cache;
            _bot = bot;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var chatId = e.CallbackQuery.Message.Chat.Id;
            _cache.ByUser.TryGetValue(chatId, out var models);

            if (!models!.TryGetValue(CallbackDataModel.FromCallbackData(e.CallbackQuery.Data).NotifyId, out var model))
            {
                await _bot.SendTextMessageAsync(chatId, "Напоминалка была удалена");
                return;
            }

            var markup = new InlineKeyboardMarkup(new[]{
                new InlineKeyboardButton
                {
                    Text = "Удалить",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.Remove, model!.NotifyId),
                },
                new InlineKeyboardButton
                {
                    Text = "Редактировать",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.EditEntry, model!.NotifyId),
                }
            });

            await _bot.SendTextMessageAsync(chatId, $"{model.ToTelegramChat()}", ParseMode.Html, replyMarkup: markup);
            await _bot.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);
        }
    }
}
