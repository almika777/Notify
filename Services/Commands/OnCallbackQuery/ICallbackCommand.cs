using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Services.Commands.OnCallbackQuery
{
    public interface ICallbackCommand
    {
        Task Execute(object? sender, CallbackQueryEventArgs e);
    }
}
