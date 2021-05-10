using Services.Services;
using Services.Services.IoServices;
using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyStepHandlers
    {
        public NotifyFrequencyStep FrequencyStep { get; }
        public NotifyReadyStep ReadyStep { get; }
        public NotifyDateStep DateStep { get; }

        public NotifyStepHandlers(TelegramBotClient bot, INotifyWriter writer, NotifyCacheService cache)
        {
            FrequencyStep = new NotifyFrequencyStep(bot);
            ReadyStep = new NotifyReadyStep(bot, cache, writer);
            DateStep = new NotifyDateStep(bot, cache);
        }
    }
}
