using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using Services.Helpers;
using Services.Services.IoServices;
using System;
using System.Threading.Tasks;
using Common.Common.Enum;
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

        public async void OnMessage(object? sender, MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;

            if (_messageCommandRepository.IsCommand(e.Message.Text))
            {
                await _messageCommandRepository.Execute(e.Message.Text, chatId);
                return;
            }

            try
            {
                if (await CheckEditCache(e, chatId)) return;

                var notifyModel = await CreateOrUpdateNotify(e, chatId);

                await _bot.SendTextMessageAsync(chatId, _notifyModifier.GetNextStepMessage(notifyModel));
            }
            catch (FormatException exception)
            {
                await _bot.SendTextMessageAsync(chatId, exception.Message);
                _logger.LogError("Чет не форматнулось", exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Чет поломалось");
            }
        }

        private async Task<NotifyModel> CreateOrUpdateNotify(MessageEventArgs e, long chatId)
        {
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
                    await _handler.Write(notifyModel);
                    break;
            }

            return notifyModel;
        }

        private async Task<bool> CheckEditCache(MessageEventArgs e, long chatId)
        {
            if (!_cache.EditCache.TryGetValue(chatId, out var value)) return false;

            if (await EditNotify(e, chatId, value.FieldType))
                await _bot.SendTextMessageAsync(chatId, "Все гуд");

            return true;

        }

        private async Task<bool> EditNotify(MessageEventArgs e, long chatId, EditField field)
        {
            try
            {
                _cache.ByUser.TryGetValue(chatId, out var models);
                models!.TryGetValue(_cache.EditCache[chatId].NotifyId, out var editedModel);

                switch (field)
                {
                    case EditField.Name: editedModel!.ChangeName(e.Message.Text.Trim()); break;
                    case EditField.Date:
                        {
                            DateTimeOffset.TryParse(e.Message.Text.Trim(), out var date);
                            editedModel!.ChangeDate(date);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(field), field, null);
                }
                return await _editor.Edit(chatId, editedModel);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Чет не радактируется");
                return false;
            }
        }
    }
}
