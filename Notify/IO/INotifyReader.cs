using System;
using Notify.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notify.IO
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyReader
    {
        public Task<IEnumerable<NotifyModel>> ReadAll();
        public Task<NotifyModel> Read(long chatId, Guid notifyId);
    }
}
