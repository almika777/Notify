using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Services.Commands.OnMessage.AdminCommands
{
    public interface IAdminMessageCommand
    {
        public Task Execute(MessageEventArgs args);
    }
}
