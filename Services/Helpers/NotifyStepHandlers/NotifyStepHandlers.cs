using Microsoft.Extensions.Logging;
using Services.Cache;
using Services.IoServices;
using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyStepHandlers
    {
        public NotifyFrequencyStep FrequencyStep { get; }
        public NotifyReadyStep ReadyStep { get; }
        public NotifyDateStep DateStep { get; }

        public NotifyStepHandlers(
            TelegramBotClient bot, 
            INotifyWriter writer, 
            INotifyEditor editor, 
            NotifyCacheService cache, 
            ILoggerFactory loggerFactory)
        {
            FrequencyStep = new NotifyFrequencyStep(bot);
            ReadyStep = new NotifyReadyStep(bot, cache, writer, editor, loggerFactory);
            DateStep = new NotifyDateStep(bot, cache);
        }
    }
}
