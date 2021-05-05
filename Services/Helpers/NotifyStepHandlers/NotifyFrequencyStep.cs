using Common.Common;
using Common.Common.CallbackModels;
using Common.Common.Enum;
using System.Threading.Tasks;
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

        public async Task Execute(long chatId, NotifyModel notifyModel)
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
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Hour),
                        Text = "Каждый час"
                    },
                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Day),
                        Text = "Каждый день"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Week),
                        Text = "Каждую неделю"
                    },
                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Month),
                        Text = "Каждый месяц"
                    },
                    new InlineKeyboardButton
                    {
                        CallbackData = CallbackFrequency.ToCallBack(FrequencyType.Custom),
                        Text = "Укажите свой период"
                    }
                }
            });

            await _bot.SendTextMessageAsync(chatId, notifyModel.GetNextStepMessage(), replyMarkup: markup);
        }
    }
}
