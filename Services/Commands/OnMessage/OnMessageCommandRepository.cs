using System;
using Common;
using Services.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Services.Commands.OnMessage
{
    public class OnMessageCommandRepository
    {
        public IDictionary<string, IMessageCommand> OnMessageCommands { get; } = new Dictionary<string, IMessageCommand>();

        public OnMessageCommandRepository(NotifyCacheService cache, TelegramBotClient bot)
        {
            OnMessageCommands.Add(BotCommands.OnMessage.StartCommand, new ReplyToStartCommand(bot));
            OnMessageCommands.Add(BotCommands.OnMessage.ShowNotificationCommand, new ShowNotificationsCommand(cache, bot));
        }

        public Task Execute(string command, long chatId)
        {
            if (!OnMessageCommands.ContainsKey(command)) throw new ArgumentException("Нет такой команды, дружок");

            return OnMessageCommands[command].Execute(chatId);
        }

        public bool IsCommand(string message) => OnMessageCommands.ContainsKey(message.Trim());
    }
}
