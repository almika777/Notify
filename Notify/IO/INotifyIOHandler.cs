using Notify.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notify.IO
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyIOHandler
    {
        public Task Write(NotifyModel model);
        public Task<IEnumerable<NotifyModel>> Read(long chatId);
        public Task<NotifyModel> Read(Guid notifyId);
    }
}
