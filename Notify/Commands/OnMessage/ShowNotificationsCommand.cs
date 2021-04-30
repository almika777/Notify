using Notify.Common;
using Notify.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notify.Commands.OnMessage
{
    public class ShowNotificationsCommand : IMessageCommand
    {
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;

        private const int InlineButtonNumber = 2;
        public ShowNotificationsCommand(NotifyCacheService cache, TelegramBotClient bot)
        {
            _cache = cache;
            _bot = bot;
        }

        public async Task Execute(long chatId)
        {
            _cache.ByUser.TryGetValue(chatId, out var userModels);

            if (userModels == null || userModels.Count == 0)
            {
                await _bot.SendTextMessageAsync(chatId, "У вас нет напоиманалок");
                return;
            }

            var buttons = new List<IEnumerable<InlineKeyboardButton>>();
            var skip = 0;

            for (var i = 0; i < userModels.Count / InlineButtonNumber + 1; i++)
            {
                buttons.Add(userModels.Skip(skip).Take(InlineButtonNumber).Select(x => new InlineKeyboardButton
                {
                    Text = x.Value.Name,
                    CallbackData = CallbackDataModel
                        .ToCallbackData(new CallbackDataModel("показать", x.Key))
                }));

                skip += InlineButtonNumber;
            }

            await _bot.SendTextMessageAsync(chatId, "Ваши напоминалки",
                replyMarkup: new InlineKeyboardMarkup(buttons));
        }
    }
}
