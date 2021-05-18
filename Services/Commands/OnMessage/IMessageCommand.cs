using System.Threading.Tasks;

namespace Services.Commands.OnMessage
{
    public interface IMessageCommand
    {
        public Task Execute(long chatId);
    }
}
