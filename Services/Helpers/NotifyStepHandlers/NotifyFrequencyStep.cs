using System.Threading.Tasks;
using Common.CallbackModels;
using Common.Enum;
using Common.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyFrequencyStep : INotifyStep
    {
        private readonly TelegramBotClient _bot;

        public NotifyFrequencyStep(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task Execute(long chatId, NotifyModel notifyModelModel)
        {
            var markup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Once),
                        Text = "Один раз"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Minute),
                        Text = "Каждую минуту"
                    },

                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Hour),
                        Text = "Каждый час"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Day),
                        Text = "Каждый день"
                    },
                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Week),
                        Text = "Каждую неделю"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Custom),
                        Text = "Укажите свой период"
                    }
                }
            });

            await _bot.SendTextMessageAsync(chatId, notifyModelModel.GetNextStepMessage(), replyMarkup: markup);
        }
    }
}
