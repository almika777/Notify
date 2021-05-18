using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyWriter
    {
        public Task Write(NotifyModel model);
        public Task Write(IEnumerable<NotifyModel> models);
    }
}
