﻿using Common;
using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnCallbackQuery.Edit;
using Services.Commands.OnMessage;
using Services.Services;
using Services.Services.IoServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnCallbackQuery
{
    public class OnCallbackCommandRepository
    {
        public IDictionary<string, ICallbackCommand> OnCallbackCommands { get; } = new Dictionary<string, ICallbackCommand>();
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly NotifyCacheService _cache;
        private readonly TelegramBotClient _bot;
        private readonly INotifyRemover _fileRemover;
        private readonly NotifyModifier _modifier;
        private readonly NotifyStepHandlers _stepHandlers;
        private readonly ILogger<OnCallbackCommandRepository> _logger;

        public OnCallbackCommandRepository(
            OnMessageCommandRepository messageCommandRepository,
            NotifyCacheService cache,
            TelegramBotClient bot,
            INotifyRemover fileRemover,
            NotifyModifier modifier,
            NotifyStepHandlers stepHandlers,
            ILogger<OnCallbackCommandRepository> logger)
        {
            _messageCommandRepository = messageCommandRepository;
            _cache = cache;
            _bot = bot;
            _fileRemover = fileRemover;
            _modifier = modifier;
            _stepHandlers = stepHandlers;
            _logger = logger;
            Init();
        }

        public Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            try
            {
                var callbackDataParams = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
                var command = callbackDataParams.Command.ToLower().Trim().Split(CommonResource.Separator);
                return OnCallbackCommands[command[0]].Execute(sender, e);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Упс");
                return Task.CompletedTask;
            }
        }

        private void Init()
        {
            OnCallbackCommands.Add(BotCommands.OnCallback.Show, new ShowOnCallback(_cache, _bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.Remove, new RemoveOnCallback(_messageCommandRepository, _fileRemover, _cache, _bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.EditEntry, new EditOnCallback(_messageCommandRepository, _fileRemover, _cache, _bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.SetFrequency, new SetFrequencyOnCallback(_cache, _bot, _modifier, _stepHandlers));
            InitEditors();
        }

        private void InitEditors()
        {
            var editor = new EditNotifyOnCallback(_cache, _bot);

            OnCallbackCommands.Add(BotCommands.OnCallback.EditNotifyName, editor);
            OnCallbackCommands.Add(BotCommands.OnCallback.EditNotifyDate, editor);
        }
    }
}
