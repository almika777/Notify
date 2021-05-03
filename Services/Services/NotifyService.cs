using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using System;
using System.Threading.Tasks;
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
        private readonly INotifyEditor _editor;
        private readonly NotifyCacheService _cache;
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly OnCallbackCommandRepository _callbackCommandRepository;
        private readonly NotifyModifier _notifyModifier;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(
            TelegramBotClient bot,
            INotifyWriter handler,
            INotifyEditor editor,
            NotifyCacheService cache,
            OnMessageCommandRepository messageCommandRepository,
            OnCallbackCommandRepository callbackCommandRepository,
            NotifyModifier notifyModifier,
            ILogger<NotifyService> logger)
        {
            _bot = bot;
            _handler = handler;
            _editor = editor;
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
            var chatId = e.Message.Chat.Id;
            if (_messageCommandRepository.IsCommand(e.Message.Text))
            {
                _messageCommandRepository.Execute(e.Message.Text, chatId);
                return;
            }

            try
            {
                if (_cache.EditName.ContainsKey(chatId))
                {
                    EditNotifyName(e, chatId);
                    return;
                }

                _cache.InProgressNotifications.TryGetValue(chatId, out var model);
                var notifyModel = _notifyModifier.CreateOrUpdate(model, e);

                switch (notifyModel!.NextStep)
                {
                    case NotifyStep.Date:
                        _cache.InProgressNotifications.TryAdd(chatId, notifyModel);
                        break;
                    case NotifyStep.Ready:
                        _cache.TryRemoveFromCurrent(notifyModel);
                        _cache.TryAddToMemory(notifyModel);
                        _handler.Write(notifyModel);
                        break;
                }

                _bot.SendTextMessageAsync(chatId, _notifyModifier.GetNextStepMessage(notifyModel));
            }
            catch (FormatException exception)
            {
                _bot.SendTextMessageAsync(chatId, exception.Message);
                _logger.LogError("Чет не форматнулось", exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Чет поломалось");
            }
        }

        private async Task EditNotifyName(MessageEventArgs e, long chatId)
        {
            _cache.ByUser.TryGetValue(chatId, out var models);
            models.TryGetValue(_cache.EditName[chatId], out var editedModel);
            editedModel.ChangeName(e.Message.Text.Trim());

            await _editor.Edit(chatId, editedModel);
        }
    }
}
