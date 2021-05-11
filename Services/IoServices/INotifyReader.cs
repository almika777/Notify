using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyReader
    {
        public Task<Notify[]> ReadAll(long chatId);
        public Task<Notify> Read(long chatId, Guid notifyId);
    }
}
