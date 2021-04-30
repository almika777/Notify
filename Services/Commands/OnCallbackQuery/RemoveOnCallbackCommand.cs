using Common.Common;
using Services.Services;
using Services.Services.IoServices;
using System.Threading.Tasks;
using Common;
using Services.Commands.OnMessage;
using Telegram.Bot;
using Telegram.Bot.Args;
// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery
{
    public class RemoveOnCallbackCommand : ICallbackCommand
    {
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly INotifyRemover _notifyRemover;
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public RemoveOnCallbackCommand(
            OnMessageCommandRepository messageCommandRepository, 
            INotifyRemover notifyRemover, 
            NotifyCacheService cache, 
            TelegramBotClient bot)
        {
            _messageCommandRepository = messageCommandRepository;
            _notifyRemover = notifyRemover;
            _bot = bot;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var key = e.CallbackQuery.Message.Chat.Id;
            var callbackData = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);

            if (!_cache.ByUser.TryGetValue(key, out var models)) return;

            models.TryGetValue(callbackData.NotifyId, out var model);

            if (models!.ContainsKey(callbackData.NotifyId)) models.Remove(callbackData.NotifyId);

            await _notifyRemover.Remove(model);
            await _bot.DeleteMessageAsync(key, e.CallbackQuery.Message.MessageId);
            await _messageCommandRepository.Execute(BotCommands.ShowNotificationCommand, key);
        }
    }
}
