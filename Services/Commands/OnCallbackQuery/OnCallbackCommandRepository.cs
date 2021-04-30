using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnMessage;
using Services.Services;
using Services.Services.IoServices;
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
            INotifyRemover remover,
            ILogger<OnCallbackCommandRepository> logger)
        {
            _logger = logger;
            OnCallbackCommands.Add(BotCommands.ShowCallbackDataCommand, new ShowOnCallbackCommand(cache, bot));
            OnCallbackCommands.Add(BotCommands.RemoveCommand, new RemoveOnCallbackCommand(messageCommandRepository, remover, cache, bot));
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
    }
}
