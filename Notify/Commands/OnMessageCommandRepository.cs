﻿using Notify.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands
{
    public class OnMessageCommandRepository
    {
        public IDictionary<string, IMessageCommand> OnMessageCommands { get; } = new Dictionary<string, IMessageCommand>();

        public OnMessageCommandRepository(NotifyCacheService cache, TelegramBotClient bot)
        {
            OnMessageCommands.Add("/start", new ReplyToStartCommand(bot));
            OnMessageCommands.Add("/show", new ShowNotificationsCommand(cache, bot));
        }

        public Task Execute(string command, MessageEventArgs e) => OnMessageCommands[command].Execute(e);

        public bool IsCommand(string message) => OnMessageCommands.ContainsKey(message.Trim());
    }
}