using Common.Common;
using System.Threading.Tasks;

namespace Services.Helpers.NotifyStepHandlers
{
    public interface INotifyStep
    {
        Task Execute(long chatId, NotifyModel notifyModel);
    }
}
