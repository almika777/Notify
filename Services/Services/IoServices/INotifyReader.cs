using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Services.Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyReader
    {
        public Task<IEnumerable<Notify>> ReadAll(long chatId);
        public Task<Notify> Read(long chatId, Guid notifyId);
    }
}
