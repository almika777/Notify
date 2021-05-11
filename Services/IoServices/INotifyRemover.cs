using System.Threading.Tasks;
using Common.Models;

namespace Services.IoServices
{
    // ReSharper disable once InconsistentNaming
    public interface INotifyRemover
    {
        public Task<bool> Remove(Notify model);
    }
}
