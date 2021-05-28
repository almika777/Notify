using AutoMapper;
using Common.Models;
using Context;
using Context.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyEditor : BaseNotifySqlService, INotifyEditor
    {
        private readonly NotifyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<NotifyEditor> _logger;

        public NotifyEditor(NotifyDbContext context, IMapper mapper, ILogger<NotifyEditor> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Edit(NotifyModel model)
        {

            try
            {
                var entity = _mapper.Map<Notify>(model);
                _context.Update(entity);
                await _context.SaveChangesAsync();

                DetachEntity(entity, _context);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ну не редактируется, блин");
                return false;
            }
        }
    }
}
