using AutoMapper;
using Common.Models;
using Context;
using Context.Entities;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyRemover : INotifyRemover
    {
        private readonly NotifyDbContext _context;
        private readonly IMapper _mapper;

        public NotifyRemover(NotifyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Remove(NotifyModel model)
        {
            _context.Notifies.Remove(_mapper.Map<Notify>(model));
            await _context.SaveChangesAsync();
            return false;
        }
    }
}
