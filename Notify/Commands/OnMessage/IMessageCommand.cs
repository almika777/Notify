using System.Threading.Tasks;

namespace Notify.Commands.OnMessage
{
    public interface IMessageCommand
    {
        public Task Execute(long chatId);
    }
}
