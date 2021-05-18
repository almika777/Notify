using System.Threading.Tasks;
using Telegram.Bot;

namespace Services.Commands.OnMessage.AdminCommands
{
    public class BotMembers : IMessageCommand
    {
        private readonly TelegramBotClient _bot;

        public BotMembers(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task Execute(long chatId)
        {
            var members = await _bot.GetChatMembersCountAsync(chatId);
            await _bot.SendTextMessageAsync(chatId, members.ToString());
        }
    }
}
