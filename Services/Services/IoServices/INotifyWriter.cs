using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Services.Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyWriter
    {
        public Task Write(NotifyModel model);
        public Task Write(IEnumerable<NotifyModel> models);
    }
}
