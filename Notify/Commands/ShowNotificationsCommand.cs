using Notify.Common;
using Notify.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notify.Commands
{
    public class ShowNotificationsCommand : IMessageCommand

    {
        private readonly INotifyIOHandler _handler;
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;

        public ShowNotificationsCommand(INotifyIOHandler handler, NotifyCacheService cache, TelegramBotClient bot)
        {
            _handler = handler;
            _cache = cache;
            _bot = bot;
        }

        public async Task Execute(MessageEventArgs e)
        {
            var modelsInCacheExists = _cache.ByUser.TryGetValue(e.Message.Chat.Id, out var userModels);
            var models = modelsInCacheExists ? userModels : (await ReadFromFile(e)).ToList();

            if (models == null || models.Count == 0)
            {
                await _bot.SendTextMessageAsync(e.Message.Chat.Id, "У вас нет напоиманалок");
                return;
            }

            var buttons = models.Select(x => new InlineKeyboardButton
            {
                Text = $@"{x.Name} | {x.Date}",
                CallbackData = x.NotifyId.ToString()
            });

            await _bot.SendTextMessageAsync(e.Message.Chat.Id, "Ваши напоминалки",
                replyMarkup: new InlineKeyboardMarkup(buttons));

            if (modelsInCacheExists)
                _cache.ByUser.TryAdd(e.Message.Chat.Id, models);
        }

        private async Task<IEnumerable<NotifyModel>> ReadFromFile(MessageEventArgs e)
            => await _handler.Read(e.Message.Chat.Id);
    }
}
