using AutoMapper;
using Common.Models;
using Context;
using Context.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyWriter : INotifyWriter
    {
        private readonly NotifyDbContext _context;
        private readonly IMapper _mapper;

        public NotifyWriter(NotifyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Write(NotifyModel model)
        {
            var entity = _mapper.Map<Notify>(model);
            await _context.Notifies.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Write(IEnumerable<NotifyModel> models)
        {
            await _context.Notifies.AddRangeAsync(_mapper.Map<IEnumerable<Notify>>(models));
            await _context.SaveChangesAsync();
        }
    }
}
