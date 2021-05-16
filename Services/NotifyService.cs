using Common;
using Common.Enum;
using Microsoft.Extensions.Logging;
using Services.Cache;
using Services.Commands.OnCallbackQuery;
using Services.Commands.OnMessage;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using Services.IoServices;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services
{
    public class NotifyService
    {
        private readonly TelegramBotClient _bot;
        private readonly INotifyEditor _editor;
        private readonly NotifyCacheService _cache;
        private readonly NotifyStepHandlers _stepHandlers;
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly OnCallbackRepository _callbackRepository;
        private readonly NotifyModifier _notifyModifier;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(
            TelegramBotClient bot,
            INotifyEditor editor,
            NotifyCacheService cache,
            NotifyStepHandlers stepHandlers,
            OnMessageCommandRepository messageCommandRepository,
            OnCallbackRepository callbackRepository,
            NotifyModifier notifyModifier,
            ILogger<NotifyService> logger)
        {
            _bot = bot;
            _editor = editor;
            _cache = cache;
            _stepHandlers = stepHandlers;
            _messageCommandRepository = messageCommandRepository;
            _callbackRepository = callbackRepository;
            _notifyModifier = notifyModifier;
            _logger = logger;
        }

        public void OnCallbackQuery(object? sender, CallbackQueryEventArgs e)
        {
            _callbackRepository.Execute(sender, e).ConfigureAwait(false).GetAwaiter().GetResult();
            _bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
        }

        public async void OnMessage(object? sender, MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;

            if (await CommandsProcessing(e: e, chatId: chatId)) return;

            try
            {
                if (await TryEditNotify(e, chatId)) return;

                await CreateOrUpdateNotify(e);
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

        private async Task<bool> CommandsProcessing(MessageEventArgs e, long chatId)
        {
            if (_messageCommandRepository.IsCommand(e.Message.Text))
            {
                await _messageCommandRepository.Execute(e.Message.Text, chatId);
                return true;
            }

            if (!_messageCommandRepository.IsAdminCommand(e.Message.Text) || CommonResource.AdminId != 285783010)
                return false;

            await _messageCommandRepository.AdminExecute(e.Message.Text, chatId);

            return true;
        }

        private async Task CreateOrUpdateNotify(MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;
            _cache.InProgressNotifications.TryGetValue(chatId, out var model);

            var notifyModel = _notifyModifier.CreateOrUpdate(model!, e.Message.Text);

            switch (notifyModel!.NextStep)
            {
                case NotifyStep.Date:
                    await _stepHandlers.DateStep.Execute(chatId, notifyModel); return;
                case NotifyStep.Frequency:
                    await _stepHandlers.FrequencyStep.Execute(chatId, notifyModel); return;
                case NotifyStep.Ready:
                    await _stepHandlers.ReadyStep.Execute(chatId, notifyModel); return;
            }
        }

        private async Task<bool> TryEditNotify(MessageEventArgs e, long chatId)
        {
            if (!_cache.EditCache.TryGetValue(chatId, out var value)) return false;
            _cache.ByUser.TryGetValue(chatId, out var models);

            try
            {
                models!.TryGetValue(_cache.EditCache[chatId].NotifyId, out var editedModel);
                _notifyModifier.EditNotify(e, editedModel!, value.FieldType);

                if (_cache.EditCache.TryRemove(chatId, out _))
                {
                    await _bot.SendTextMessageAsync(chatId, "Все гуд");
                }

                return await _editor.Edit(chatId, editedModel!);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Чет не радактируется");
                return false;
            }
        }
    }
}
