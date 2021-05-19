using Common.Models;
using System;
using System.Threading.Tasks;

namespace Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyReader
    {
        public Task<NotifyModel[]> ReadAll(long chatId);
        public Task<NotifyModel> Read(long chatId, Guid notifyId);
    }
}
