using Common.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class NotifyCacheService
    {
        public bool IsInitialized { get; private set; }

        public ConcurrentDictionary<long, IDictionary<Guid, NotifyModel>> ByUser { get; }
            = new ConcurrentDictionary<long, IDictionary<Guid, NotifyModel>>();

        public ConcurrentDictionary<long, Guid> EditName { get; }
            = new ConcurrentDictionary<long, Guid>();

        public readonly ConcurrentDictionary<long, NotifyModel> InProgressNotifications =
            new ConcurrentDictionary<long, NotifyModel>();


        private readonly Configuration _config;
        private readonly ILogger<NotifyCacheService> _logger;

        public NotifyCacheService(IOptions<Configuration> config, ILogger<NotifyCacheService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task Initialize()
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)
                || !Directory.Exists(_config.CacheFolder)
                || IsInitialized) return;

            var directoryInfo = new DirectoryInfo(_config.CacheFolder);
            var files = directoryInfo.GetFiles();

            try
            {
                var readFilesTasks = files.ToDictionary(
                    fileInfo => long.Parse(Path.GetFileNameWithoutExtension(fileInfo.Name)),
                    fileInfo => File.ReadAllLinesAsync(fileInfo!.FullName));

                await Task.WhenAll(readFilesTasks.Values);

                foreach (var (key, value) in readFilesTasks)
                {
                    ByUser.TryAdd(key, value.Result
                        .Where(x => x.Length > 1)
                        .Select(NotifyModel.FromString)
                        .ToDictionary(x => x.NotifyId));
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
            if (!ByUser.ContainsKey(model.ChatId))
            {
                return ByUser.TryAdd(model.ChatId, new Dictionary<Guid, NotifyModel> { { model.NotifyId, model } });
            }

            ByUser[model.ChatId].Add(model.NotifyId, model);
            return true;
        }

        public bool TryRemoveFromCurrent(NotifyModel model)
        {
            return InProgressNotifications.TryRemove(new KeyValuePair<long, NotifyModel>(model.ChatId, model));
        }
    }
}
