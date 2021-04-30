using Notify.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands.OnMessage
{
    public class OnMessageCommandRepository
    {
        public IDictionary<string, IMessageCommand> OnMessageCommands { get; } = new Dictionary<string, IMessageCommand>();

        public OnMessageCommandRepository(NotifyCacheService cache, TelegramBotClient bot)
        {
            OnMessageCommands.Add(BotCommands.StartCommand, new ReplyToStartCommand(bot));
            OnMessageCommands.Add(BotCommands.ShowNotificationCommand, new ShowNotificationsCommand(cache, bot));
        }

        public Task Execute(string command, long chatId) => OnMessageCommands[command].Execute(chatId);

        public bool IsCommand(string message) => OnMessageCommands.ContainsKey(message.Trim());
    }
}
