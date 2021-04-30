using System;
using System.Threading.Tasks;
using Common;
using Common.Common;
using Services.Services;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Services.Commands.OnCallbackQuery
{
    public class ShowOnCallbackCommand : ICallbackCommand
    {
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;

        public ShowOnCallbackCommand(NotifyCacheService cache, TelegramBotClient bot)
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
                    CallbackData = string.Join(CommonResource.Separator,"удалить", model!.NotifyId),
                },
                new InlineKeyboardButton
                {
                    Text = "Редактировать",
                    CallbackData = string.Join(CommonResource.Separator,"редактировать", model.NotifyId),
                }
            });

            await _bot.SendTextMessageAsync(chatId, $"{model.Name}{Environment.NewLine}Дата: {model.Date:g}", replyMarkup: markup);
            await _bot.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);
        }
    }
}
