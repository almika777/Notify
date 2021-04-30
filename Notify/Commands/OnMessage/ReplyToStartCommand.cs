using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Notify.Commands.OnMessage
{
    public class ReplyToStartCommand : IMessageCommand
    {
        private readonly TelegramBotClient _bot;

        public ReplyToStartCommand(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public  Task Execute(long chatId)
        {
            return _bot.SendTextMessageAsync(chatId,
                $@"Привет, если ты вечно что-то забываешь, то я помогу тебе. 
                {Environment.NewLine}Хочешь добавить напоминалку просто жми");
        }
    }
}
