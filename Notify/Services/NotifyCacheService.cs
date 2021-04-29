using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Common;

namespace Notify.Services
{
    public class NotifyCacheService
    {
        public bool IsInitialized { get; private set; }

        public ConcurrentDictionary<long, List<NotifyModel>> ByUser { get; } = new ConcurrentDictionary<long, List<NotifyModel>>();

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
                    ByUser.TryAdd(key, value.Result.Where(x => x.Length > 1).Select(NotifyModel.FromString).ToList());
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
                return ByUser.TryAdd(model.ChatId, new List<NotifyModel> {model});
            }

            ByUser[model.ChatId].Add(model);
            return true;
        }

        public bool TryRemoveFromCurrent(NotifyModel model)
        {
            if (model.CurrentState != NotifyState.Date)
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
