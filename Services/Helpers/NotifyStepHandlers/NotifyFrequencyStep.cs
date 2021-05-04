using Common.Common;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyFrequencyStep : INotifyStepHandler
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyModifier _notifyModifier;

        public NotifyFrequencyStep(TelegramBotClient bot, NotifyModifier notifyModifier)
        {
            _bot = bot;
            _notifyModifier = notifyModifier;
        }

        public async Task Execute(long chatId, NotifyModel notifyModel)
        {
            var markup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = string.Join(CommonResource.Separator, "frequency","0"),
                        Text = "Один раз"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = "1",
                        Text = "Каждую минуту"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = "2",
                        Text = "Каждый час"
                    },
                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = "3",
                        Text = "Каждый день"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = "4",
                        Text = "Каждую неделю"
                    },
                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = "5",
                        Text = "Каждый месяц"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = "99",
                        Text = "Укажите свой период"
                    }
                }
            });

            await _bot.SendTextMessageAsync(chatId, _notifyModifier.GetNextStepMessage(notifyModel), replyMarkup: markup);
        }
    }
}
