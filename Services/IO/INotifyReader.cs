using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Services.IO
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyReader
    {
        public Task<IEnumerable<NotifyModel>> ReadAll();
        public Task<NotifyModel> Read(long chatId, Guid notifyId);
    }
}
