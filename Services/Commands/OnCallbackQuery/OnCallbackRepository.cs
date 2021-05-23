using Common;
using Common.CallbackModels;
using Microsoft.Extensions.Logging;
using Services.Cache;
using Services.Commands.OnCallbackQuery.Edit;
using Services.Commands.OnMessage;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using Services.IoServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnCallbackQuery
{
    public class OnCallbackRepository
    {
        public IDictionary<string, ICallbackCommand> OnCallbackCommands { get; } = new Dictionary<string, ICallbackCommand>();
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;
        private readonly INotifyRemover _fileRemover;
        private readonly NotifyModifier _modifier;
        private readonly NotifyStepHandlers _stepHandlers;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<OnCallbackRepository> _logger;

        public OnCallbackRepository(
            OnMessageCommandRepository messageCommandRepository,
            NotifyCacheService cache,
            TelegramBotClient bot,
            INotifyRemover fileRemover,
            NotifyModifier modifier,
            NotifyStepHandlers stepHandlers,
            ILoggerFactory loggerFactory)
        {
            _messageCommandRepository = messageCommandRepository;
            _cache = cache;
            _bot = bot;
            _fileRemover = fileRemover;
            _modifier = modifier;
            _stepHandlers = stepHandlers;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<OnCallbackRepository>();
            Init();
        }

        public Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            try
            {
                var callbackDataParams = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
                return OnCallbackCommands[callbackDataParams.Command].Execute(sender, e);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Упс");
                return Task.CompletedTask;
            }
        }

        private void Init()
        {
            OnCallbackCommands.Add(BotCommands.OnCallback.Show, 
                new ShowOnCallback(_cache, _bot));

            OnCallbackCommands.Add(BotCommands.OnCallback.Remove, 
                new RemoveOnCallback(_messageCommandRepository, _fileRemover, _cache, _bot));

            OnCallbackCommands.Add(BotCommands.OnCallback.EditEntry, 
                new EditOnCallback(_cache, _bot));

            OnCallbackCommands.Add(BotCommands.OnCallback.SetFrequency, 
                new SetFrequencyOnCallback(_cache, _bot, _modifier, _stepHandlers, _loggerFactory));

            InitEditors();
        }

        private void InitEditors()
        {
            var editor = new EditNotifyOnCallback(_bot, _stepHandlers, _cache);

            OnCallbackCommands.Add(BotCommands.OnCallback.Edit.NotifyName, editor);
            OnCallbackCommands.Add(BotCommands.OnCallback.Edit.NotifyDate, editor);
            OnCallbackCommands.Add(BotCommands.OnCallback.Edit.NotifyFrequency, editor);
        }
    }
}
