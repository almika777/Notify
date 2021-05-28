using Common;
using Services.Cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Commands.OnMessage.AdminCommands;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnMessage
{
    public class OnMessageCommandRepository
    {
        public IDictionary<string, IMessageCommand> OnMessageCommands { get; } = new Dictionary<string, IMessageCommand>();
        public IDictionary<string, IAdminMessageCommand> OnMessageAdminCommands { get; } = new Dictionary<string, IAdminMessageCommand>();
        public bool IsCommand(string message) => OnMessageCommands.ContainsKey(message.Trim());
        public bool IsAdminCommand(string message) => OnMessageAdminCommands.ContainsKey(message.Trim());

        private readonly TelegramBotClient _bot;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Configuration _configuration;

        public OnMessageCommandRepository(
            NotifyCacheService cache, 
            TelegramBotClient bot, 
            IOptions<Configuration> configuration,
            ILoggerFactory loggerFactory)
        {
            _bot = bot;
            _loggerFactory = loggerFactory;
            _configuration = configuration.Value;
            OnMessageCommands.Add(BotCommands.OnMessage.Start, new ReplyToStartCommand(bot));
            OnMessageCommands.Add(BotCommands.OnMessage.ShowNotification, new ShowNotificationsCommand(cache, bot));
            AddAdminCommands();
        }

        public Task Execute(string command, long chatId)
        {
            if (!OnMessageCommands.ContainsKey(command)) throw new ArgumentException("Нет такой команды, дружок");

            return OnMessageCommands[command].Execute(chatId);
        }

        public Task AdminExecute(string command, MessageEventArgs args)
        {
            if (!OnMessageAdminCommands.ContainsKey(command)) throw new ArgumentException("Нет такой команды, дружок");

            return OnMessageAdminCommands[command].Execute(args);
        }

        private void AddAdminCommands()
        {
            OnMessageAdminCommands.Add(BotCommands.OnMessage.Admin.Members, new BotMembers(_bot));
            OnMessageAdminCommands.Add(BotCommands.OnMessage.Admin.GetDatabase, 
                new GetDatabase(_bot, _configuration, _loggerFactory));
        }
    }
}
