using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Notify.Commands
{
    public interface IMessageCommand
    {
        public Task Execute(MessageEventArgs e);
    }
}
