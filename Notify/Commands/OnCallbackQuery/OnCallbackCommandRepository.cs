using Notify.Commands.OnMessage;
using Notify.Common;
using Notify.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands.OnCallbackQuery
{
    public class OnCallbackCommandRepository
    {
        public IDictionary<string, ICallbackCommand> OnCallbackCommands { get; } = new Dictionary<string, ICallbackCommand>();
        private readonly ILogger<OnCallbackCommandRepository> _logger;

        public OnCallbackCommandRepository( 
            NotifyCacheService cache, 
            TelegramBotClient bot,
            ILogger<OnCallbackCommandRepository> logger)
        {
            _logger = logger;
            OnCallbackCommands.Add(BotCommands.ShowCallbackDataCommand, new ShowOnCallbackCommand(cache, bot));
            OnCallbackCommands.Add(BotCommands.RemoveCommand, new RemoveOnCallbackCommand(cache, bot));
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
