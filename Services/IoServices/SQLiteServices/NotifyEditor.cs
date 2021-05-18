using AutoMapper;
using Common.Models;
using Context;
using Context.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Services.IoServices.SQLiteServices
{
    public class NotifyEditor : INotifyEditor
    {
        private readonly IServiceProvider _provider;
        private readonly IMapper _mapper;
        private readonly ILogger<NotifyEditor> _logger;

        public NotifyEditor(IServiceProvider provider, IMapper mapper, ILogger<NotifyEditor> logger)
        {
            _provider = provider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Edit(long chatId, NotifyModel model)
        {
            using var scope = _provider.CreateScope();

            try
            {
                var entity = _mapper.Map<Notify>(model);
                await using var context = scope.ServiceProvider.GetRequiredService<NotifyDbContext>();

                context.Update(entity);
                await context.SaveChangesAsync();

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
