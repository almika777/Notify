using AutoMapper;
using Common.Enum;
using Common.Models;
using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Cache
{
    public class NotifyCacheService
    {
        public bool IsInitialized { get; private set; }

        public ConcurrentDictionary<long, (Guid NotifyId, EditField FieldType)> EditCache { get; }
            = new ConcurrentDictionary<long, (Guid NotifyId, EditField FieldType)>();

        public ConcurrentDictionary<long, IDictionary<Guid, NotifyModel>> ByUser { get; }
            = new ConcurrentDictionary<long, IDictionary<Guid, NotifyModel>>();

        public ConcurrentDictionary<long, NotifyModel> InProgressNotifications { get; } =
            new ConcurrentDictionary<long, NotifyModel>();


        private readonly IServiceProvider _provider;
        private readonly IMapper _mapper;
        private readonly ILogger<NotifyCacheService> _logger;

        public NotifyCacheService(IServiceProvider provider, IMapper mapper, ILogger<NotifyCacheService> logger)
        {
            _provider = provider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Initialize()
        {
            if (IsInitialized) return;

            try
            {
                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<NotifyDbContext>();
                    await using (context)
                    {
                        var models = await context!.Notifies.AsNoTracking().ToArrayAsync();
                        var modelsByUser = models.GroupBy(x => x.UserId).ToDictionary(x => x.Key);

                        foreach (var (key, value) in modelsByUser)
                        {
                            ByUser.TryAdd(key, value.ToDictionary(x => x.NotifyId, x => _mapper.Map<NotifyModel>(x)));
                        }
                    }
                }

                IsInitialized = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }
        }

        public bool TryAddToMemory(NotifyModel model)
        {
            if (!ByUser.ContainsKey(model.UserId))
            {
                return ByUser.TryAdd(model.UserId, new Dictionary<Guid, NotifyModel> { { model.NotifyId, model } });
            }

            if (EditCache.ContainsKey(model.UserId))
            {
                ByUser[model.UserId].Remove(model.NotifyId);
                ByUser[model.UserId].Add(model.NotifyId, model);
            }
            else
            {
                ByUser[model.UserId].Add(model.NotifyId, model);
            }

            return true;
        }

        public bool TryRemoveFromCurrent(NotifyModel model)
        {
            return InProgressNotifications.TryRemove(new KeyValuePair<long, NotifyModel>(model.UserId, model));
        }
    }
}
