using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyStepHandlers
    {
        public NotifyFrequencyStep FrequencyStep { get; }

        public NotifyStepHandlers(TelegramBotClient bot, NotifyModifier notifyModifier)
        {
            FrequencyStep = new NotifyFrequencyStep(bot, notifyModifier);
        }
    }
}
