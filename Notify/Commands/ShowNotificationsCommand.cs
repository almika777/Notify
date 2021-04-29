using System;
using System.Collections.Generic;
using Notify.Services;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notify.Commands
{
    public class ShowNotificationsCommand : IMessageCommand
    {
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;

        private const int _inlineButtonNumber = 2;
        public ShowNotificationsCommand(NotifyCacheService cache, TelegramBotClient bot)
        {
            _cache = cache;
            _bot = bot;
        }

        public async Task Execute(MessageEventArgs e)
        {
            var modelsInCacheExists = _cache.ByUser.TryGetValue(e.Message.Chat.Id, out var userModels);
            var buttons = new List<IEnumerable<InlineKeyboardButton>>();
            if (userModels == null || userModels.Count == 0)
            {
                await _bot.SendTextMessageAsync(e.Message.Chat.Id, "У вас нет напоиманалок");
                return;
            }

            var skip = 0;

            for (var i = 0; i < userModels.Count / _inlineButtonNumber; i++)
            {
                buttons.Add(userModels.Skip(skip).Take(_inlineButtonNumber).Select(x => new InlineKeyboardButton
                {
                    Text = $@"{x.Name} | {x.Date:g}",
                    CallbackData = x.NotifyId.ToString()
                }));

                skip += _inlineButtonNumber;
            }

            await _bot.SendTextMessageAsync(e.Message.Chat.Id, "Ваши напоминалки",
                replyMarkup: new InlineKeyboardMarkup(buttons));

            if (modelsInCacheExists)
                _cache.ByUser.TryAdd(e.Message.Chat.Id, userModels);
        }
    }
}
