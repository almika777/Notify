using Notify.Commands.OnMessage;
using Notify.Common;
using Notify.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands.OnCallbackQuery
{
    public class OnCallbackCommandRepository
    {
        public IDictionary<string, ICallbackCommand> OnCallbackCommands { get; } = new Dictionary<string, ICallbackCommand>();

        public OnCallbackCommandRepository( 
            NotifyCacheService cache, 
            TelegramBotClient bot)
        {
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
                Console.WriteLine(exception);
            }

            return null;
        }
    }
}
