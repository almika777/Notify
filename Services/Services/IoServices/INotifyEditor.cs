using System;
using System.Threading.Tasks;
using Common.Common;

namespace Services.Services.IoServices
{
    public interface INotifyEditor
    {
        Task<bool> Edit(long chatId, NotifyModel model);
    }
}
