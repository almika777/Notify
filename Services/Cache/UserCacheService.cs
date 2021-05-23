using Common.Models;
using System.Collections.Concurrent;

namespace Services.Cache
{
    public class UserCacheService
    {
        public ConcurrentDictionary<long, ChatUserModel> Users { get; } =
            new ConcurrentDictionary<long, ChatUserModel>();
    }
}
