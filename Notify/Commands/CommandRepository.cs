using System;
using System.Collections.Generic;
using Notify.IO;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands
{
    public class CommandRepository
    {
        private readonly INotifyIOHandler _handler;
        public IDictionary<CommandType,IMessageCommand> OnMessageCommands { get; } = new Dictionary<CommandType, IMessageCommand>();

        public CommandRepository(INotifyIOHandler handler, NotifyCache cache, TelegramBotClient bot)
        {
            _handler = handler;
            OnMessageCommands.Add(CommandType.ReplyToStart, new ReplyToStartCommand(bot));
            OnMessageCommands.Add(CommandType.ShowNotifications, new ShowNotificationsCommand(handler,cache, bot));
        }

        public void Execute(CommandType commandType, MessageEventArgs e)
        {
            if (!OnMessageCommands.ContainsKey(commandType)) throw new ArgumentException("Неверно указан тип комманды");

            OnMessageCommands[commandType].Execute(e);
        }
    }
}
