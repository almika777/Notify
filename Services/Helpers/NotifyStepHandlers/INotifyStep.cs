using System.Threading.Tasks;
using Common.Models;

namespace Services.Helpers.NotifyStepHandlers
{
    public interface INotifyStep
    {
        Task Execute(long chatId, Notify notifyModel);
    }
}
