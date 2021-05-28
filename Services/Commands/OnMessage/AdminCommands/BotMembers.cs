using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnMessage.AdminCommands
{
    public class BotMembers : IAdminMessageCommand
    {
        private readonly TelegramBotClient _bot;

        public BotMembers(TelegramBotClient bot)
        {
            _bot = bot;
        }


        public async Task Execute(MessageEventArgs args)
        {
            var chatId = args.Message.Chat.Id;
            var members = await _bot.GetChatMembersCountAsync(chatId);
            await _bot.SendTextMessageAsync(chatId, members.ToString());
        }
    }
}
