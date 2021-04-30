using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using System;
using Services.Helpers;
using Services.Services.IoServices;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Services
{
    public class NotifyService
    {
        private readonly TelegramBotClient _bot;
        private readonly INotifyWriter _handler;
        private readonly NotifyCacheService _cache;
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly OnCallbackCommandRepository _callbackCommandRepository;
        private readonly NotifyModifier _notifyModifier;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(
            TelegramBotClient bot,
            INotifyWriter handler,
            NotifyCacheService cache,
            OnMessageCommandRepository messageCommandRepository,
            OnCallbackCommandRepository callbackCommandRepository,
            NotifyModifier notifyModifier,
            ILogger<NotifyService> logger)
        {
            _bot = bot;
            _handler = handler;
            _cache = cache;
            _messageCommandRepository = messageCommandRepository;
            _callbackCommandRepository = callbackCommandRepository;
            _notifyModifier = notifyModifier;
            _logger = logger;
        }

        public void OnCallbackQuery(object? sender, CallbackQueryEventArgs e)
        {
            _callbackCommandRepository.Execute(sender, e).ConfigureAwait(false).GetAwaiter().GetResult();
            _bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
        }

        public void OnMessage(object? sender, MessageEventArgs e)
        {
            if (_messageCommandRepository.IsCommand(e.Message.Text))
            {
                _messageCommandRepository.Execute(e.Message.Text, e.Message.Chat.Id);
                return;
            }

            try
            {
                _cache.InProgressNotifications.TryGetValue(e.Message.Chat.Id, out var model);
                var notifyModel = _notifyModifier.CreateOrUpdate(model, e);

                switch (notifyModel!.NextStep)
                {
                    case NotifyStep.Date:
                        _cache.InProgressNotifications.TryAdd(e.Message.Chat.Id, notifyModel);
                        break;
                    case NotifyStep.Ready:
                        _cache.TryRemoveFromCurrent(notifyModel);
                        _cache.TryAddToMemory(notifyModel);
                        _handler.Write(notifyModel);
                        break;
                }

                _bot.SendTextMessageAsync(e.Message.Chat.Id, _notifyModifier.GetNextStepMessage(notifyModel));
            }
            catch (FormatException exception)
            {
                _bot.SendTextMessageAsync(e.Message.Chat.Id, exception.Message);
                _logger.LogError("Чет не форматнулось", exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Чет поломалось");
            }
        }
    }
}
