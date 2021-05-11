using Common.Models;
using Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyWriter : INotifyWriter
    {
        private readonly NotifyDbContext _context;

        public NotifyWriter(NotifyDbContext context)
        {
            _context = context;
        }

        public async Task Write(Notify model)
        {
            await _context.Notifies.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Write(IEnumerable<Notify> models)
        {
            await _context.Notifies.AddRangeAsync(models);
            await _context.SaveChangesAsync();
        }
    }
}
