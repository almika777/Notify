using Common.Enum;
using Common.Models;
using Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Cache
{
    public class NotifyCacheService
    {
        public bool IsInitialized { get; private set; }

        public ConcurrentDictionary<long, (Guid NotifyId, EditField FieldType)> EditCache { get; }
            = new ConcurrentDictionary<long, (Guid NotifyId, EditField FieldType)>();

        public ConcurrentDictionary<long, IDictionary<Guid, Notify>> ByUser { get; }
            = new ConcurrentDictionary<long, IDictionary<Guid, Notify>>();

        public ConcurrentDictionary<long, Notify> InProgressNotifications { get; } =
            new ConcurrentDictionary<long, Notify>();


        private readonly IServiceProvider _provider;
        private readonly ILogger<NotifyCacheService> _logger;

        public NotifyCacheService(IServiceProvider provider, ILogger<NotifyCacheService> logger)
        {
            _provider = provider;
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
                        var models = await context!.Notifies.ToArrayAsync();
                        var modelsByUser = models.GroupBy(x => x.UserId).ToDictionary(x => x.Key);

                        foreach (var (key, value) in modelsByUser)
                        {
                            ByUser.TryAdd(key, value.ToDictionary(x => x.NotifyId));
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

        public bool TryAddToMemory(Notify model)
        {
            if (!ByUser.ContainsKey(model.UserId))
            {
                return ByUser.TryAdd(model.UserId, new Dictionary<Guid, Notify> { { model.NotifyId, model } });
            }

            ByUser[model.UserId].Add(model.NotifyId, model);
            return true;
        }

        public bool TryRemoveFromCurrent(Notify model)
        {
            return InProgressNotifications.TryRemove(new KeyValuePair<long, Notify>(model.UserId, model));
        }
    }
}
