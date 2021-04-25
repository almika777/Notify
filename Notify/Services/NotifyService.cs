using Microsoft.Extensions.Logging;
using Notify.Commands;
using Notify.Helpers;
using Notify.IO;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Services
{
    public class NotifyService
    {
        private readonly TelegramBotClient _bot;
        private readonly INotifyIOHandler _handler;
        private readonly NotifyCacheService _cache;
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly NotifyModifier _notifyModifier;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(TelegramBotClient bot, INotifyIOHandler handler, NotifyCacheService cache,
            OnMessageCommandRepository messageCommandRepository, NotifyModifier notifyModifier, ILogger<NotifyService> logger)
        {
            _bot = bot;
            _handler = handler;
            _cache = cache;
            _messageCommandRepository = messageCommandRepository;
            _notifyModifier = notifyModifier;
            _logger = logger;
        }

        public void OnOnCallbackQuery(object? sender, CallbackQueryEventArgs e)
        {


        }

        public void OnMessage(object? sender, MessageEventArgs e)
        {
            if (_messageCommandRepository.IsCommand(e.Message.Text))
            {
                _messageCommandRepository.Execute(e.Message.Text, e);
                return;
            }

            try
            {
                var modelIsExist = _cache.InProgressNotifications.TryGetValue(e.Message.Chat.Id, out var model);
                var notifyModel = modelIsExist
                    ? _notifyModifier.Update(model!, e)
                    : _notifyModifier.Create(e);

                _bot.SendTextMessageAsync(e.Message.Chat.Id, _notifyModifier.GetNextStepMessage(notifyModel));
                _handler.Write(notifyModel);

                var cacheIsUpdate = modelIsExist 
                    ? _cache.TryRemove(notifyModel) 
                    : _cache.InProgressNotifications.TryAdd(e.Message.Chat.Id, notifyModel);
            }
            catch (FormatException exception)
            {
                _bot.SendTextMessageAsync(e.Message.Chat.Id, exception.Message);
            }
        }
    }
}
