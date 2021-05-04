using Common.Common;
using System.Threading.Tasks;

namespace Services.Helpers.NotifyStepHandlers
{
    public interface INotifyStepHandler
    {
        Task Execute(long chatId, NotifyModel notifyModel);
    }
}
