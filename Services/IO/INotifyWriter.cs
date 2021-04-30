using System.Threading.Tasks;
using Common.Common;

namespace Services.IO
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyWriter
    {
        public Task Write(NotifyModel model);
    }
}
