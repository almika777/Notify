using Notify.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands
{
    public class CommandRepository
    {
        public IDictionary<string, IMessageCommand> OnMessageCommands { get; } = new Dictionary<string, IMessageCommand>();

        public CommandRepository(INotifyIOHandler handler, NotifyCache cache, TelegramBotClient bot)
        {
            OnMessageCommands.Add("/start", new ReplyToStartCommand(bot));
            OnMessageCommands.Add("/show", new ShowNotificationsCommand(handler, cache, bot));
        }

        public Task Execute(string command, MessageEventArgs e)
            => IsCommand(command)
                ? Task.FromException<ArgumentException>(new ArgumentException("Неверно указан тип комманды"))
                : OnMessageCommands[command].Execute(e);


        public bool IsCommand(string message) => OnMessageCommands.ContainsKey(message.Trim());
    }
}
