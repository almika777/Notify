using Common;
using Common.Common;
using Services.Commands.OnMessage;
using Services.Services;
using Services.Services.IoServices;
using System.Threading.Tasks;
using Common.Common.Enum;
using Telegram.Bot;
using Telegram.Bot.Args;
// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery
{
    public class SetFrequencyOnCallback : ICallbackCommand
    {
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly INotifyRemover _notifyFileRemover;
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public SetFrequencyOnCallback(
            OnMessageCommandRepository messageCommandRepository,
            INotifyRemover notifyFileRemover,
            NotifyCacheService cache,
            TelegramBotClient bot)
        {
            _messageCommandRepository = messageCommandRepository;
            _notifyFileRemover = notifyFileRemover;
            _bot = bot;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var key = e.CallbackQuery.Message.Chat.Id;
            _cache.InProgressNotifications.TryGetValue(key, out var model);

            if (model!.NextStep == NotifyStep.Frequency)
            {

            }
        }
    }
}
