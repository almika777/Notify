using Notify.Common;
using System.Threading.Tasks;

namespace Notify.IO
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyWriter
    {
        public Task Write(NotifyModel model);
    }
}
