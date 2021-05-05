using Services.Services;
using Services.Services.IoServices;
using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyStepHandlers
    {
        public NotifyFrequencyStep FrequencyStep { get; }
        public NotifyReadyStep ReadyStep { get; }

        public NotifyStepHandlers(TelegramBotClient bot, INotifyWriter writer, NotifyCacheService cache)
        {
            FrequencyStep = new NotifyFrequencyStep(bot);
            ReadyStep = new NotifyReadyStep(cache, writer, bot);
        }
    }
}
