using Common.Models;
using Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyReader : INotifyReader
    {
        private readonly NotifyDbContext _context;

        public NotifyReader(NotifyDbContext context)
        {
            _context = context;
        }

        public Task<Notify[]> ReadAll(long chatId)
        {
            return _context.Notifies
                .AsNoTracking()
                .Where(x => x.UserId == chatId)
                .ToArrayAsync();
        }

        public Task<Notify> Read(long chatId, Guid notifyId)
        {
            return _context.Notifies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NotifyId == notifyId && x.UserId == chatId);
        }
    }
}
