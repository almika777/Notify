using System;
using Microsoft.Extensions.Logging;
using Notify.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace Notify
{
    public class NotifyCacheService
    {
        private readonly ILogger<NotifyCacheService> _logger;

        public readonly ConcurrentDictionary<long, List<NotifyModel>> ByUser =
            new ConcurrentDictionary<long, List<NotifyModel>>();

        public readonly ConcurrentDictionary<long, NotifyModel> InProgressNotifications =
            new ConcurrentDictionary<long, NotifyModel>();

        public NotifyCacheService(ILogger<NotifyCacheService> logger)
        {
            _logger = logger;
        }

        public async Task Initialize()
        {

        }

        public bool TryRemove(NotifyModel model)
        {
            if (model.State != NotifyState.Ready)
                return InProgressNotifications.TryRemove(new KeyValuePair<long, NotifyModel>(model.ChatId, model));


            if (ByUser.ContainsKey(model.ChatId))
            {
                ByUser[model.ChatId].Add(model);
            }
            else
            {
                ByUser.TryAdd(model.ChatId, new List<NotifyModel> { model });
            }

            return InProgressNotifications.TryRemove(new KeyValuePair<long, NotifyModel>(model.ChatId, model));
        }
    }
}
