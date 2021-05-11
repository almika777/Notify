using Common.Models;
using Context;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyRemover : INotifyRemover
    {
        private readonly NotifyDbContext _context;

        public NotifyRemover(NotifyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Remove(Notify model)
        {
            _context.Notifies.Remove(model);
            await _context.SaveChangesAsync();
            return false;
        }
    }
}
