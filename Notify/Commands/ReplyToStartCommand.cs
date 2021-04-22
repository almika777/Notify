using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify.Commands
{
    public class ReplyToStartCommand : IMessageCommand
    {
        private readonly TelegramBotClient _bot;

        public ReplyToStartCommand(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public  Task Execute(MessageEventArgs e)
        {
            return _bot.SendTextMessageAsync(e.Message.Chat.Id,
                $@"Привет, если ты вечно что-то забываешь, то я помогу тебе. 
                {Environment.NewLine}Хочешь добавить напоминалку просто жми");
        }
    }
}
