using System.Threading.Tasks;
using Common.Common;

namespace Services.Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyRemover
    {
        public Task<bool> Remove(NotifyModel model);
    }
}
