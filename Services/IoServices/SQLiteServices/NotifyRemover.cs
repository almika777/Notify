using AutoMapper;
using Common.Models;
using Context;
using Context.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyRemover : BaseNotifySqlService, INotifyRemover
    {
        private readonly NotifyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<NotifyRemover> _logger;

        public NotifyRemover(NotifyDbContext context, IMapper mapper, ILogger<NotifyRemover> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Remove(NotifyModel model)
        {
            try
            {
                var entity = _mapper.Map<Notify>(model);
                DetachEntity(entity, _context);

                _context.Notifies.Remove(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Remove");
                return false;
            }
        }
    }
}
