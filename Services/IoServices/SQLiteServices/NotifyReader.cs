using AutoMapper;
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
        private readonly IMapper _mapper;

        public NotifyReader(NotifyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<NotifyModel[]> ReadAll(long chatId)
        {
            var entities = await _context.Notifies
                .AsNoTracking()
                .Where(x => x.UserId == chatId)
                .ToArrayAsync();

            return _mapper.Map<NotifyModel[]>(entities);
        }

        public async Task<NotifyModel> Read(long chatId, Guid notifyId)
        {
            var entity =  await _context.Notifies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NotifyId == notifyId && x.UserId == chatId);

            return _mapper.Map<NotifyModel>(entity);
        }
    }
}
