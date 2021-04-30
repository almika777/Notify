using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Notify.Commands.OnCallbackQuery
{
    public interface ICallbackCommand
    {
        Task Execute(object? sender, CallbackQueryEventArgs e);
    }
}
