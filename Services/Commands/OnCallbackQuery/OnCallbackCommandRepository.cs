using Common;
using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnCallbackQuery.Edit;
using Services.Commands.OnMessage;
using Services.Services;
using Services.Services.IoServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnCallbackQuery
{
    public class OnCallbackCommandRepository
    {
        public IDictionary<string, ICallbackCommand> OnCallbackCommands { get; } = new Dictionary<string, ICallbackCommand>();
        private readonly ILogger<OnCallbackCommandRepository> _logger;

        public OnCallbackCommandRepository(
            OnMessageCommandRepository messageCommandRepository,
            NotifyCacheService cache,
            TelegramBotClient bot,
            INotifyRemover fileRemover,
            ILogger<OnCallbackCommandRepository> logger)
        {
            _logger = logger;
            Init(messageCommandRepository, cache, bot, fileRemover);
        }

        public Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            try
            {
                var callbackDataParams = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
                return OnCallbackCommands[callbackDataParams.Command.ToLower().Trim()].Execute(sender, e);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Упс");
                return Task.CompletedTask;
            }
        }

        private void Init(OnMessageCommandRepository messageCommandRepository, NotifyCacheService cache, TelegramBotClient bot,
            INotifyRemover fileRemover)
        {
            OnCallbackCommands.Add(BotCommands.OnCallback.Show, new ShowOnCallbackCommand(cache, bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.Remove, new RemoveOnCallbackCommand(messageCommandRepository, fileRemover, cache, bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.EditEntry, new EditOnCallbackCommand(messageCommandRepository, fileRemover, cache, bot));
            InitEditors(cache, bot);
        }

        private void InitEditors(NotifyCacheService cache, TelegramBotClient bot)
        {
            var editor = new EditNotifyOnCallbackCommand(cache, bot);

            OnCallbackCommands.Add(BotCommands.OnCallback.EditNotifyName, editor);
            OnCallbackCommands.Add(BotCommands.OnCallback.EditNotifyDate, editor);
        }
    }
}
