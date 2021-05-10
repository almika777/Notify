using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Enum;
using Common.Models;
using Context;

namespace Services.Services
{
    public class NotifyCacheService
    {
        public bool IsInitialized { get; private set; }

        public ConcurrentDictionary<long, (Guid NotifyId, EditField FieldType)> EditCache { get; }
            = new ConcurrentDictionary<long, (Guid NotifyId, EditField FieldType)>();

        public ConcurrentDictionary<long, IDictionary<Guid, Notify>> ByUser { get; }
            = new ConcurrentDictionary<long, IDictionary<Guid, Notify>>();


        public readonly ConcurrentDictionary<long, Notify> InProgressNotifications =
            new ConcurrentDictionary<long, Notify>();


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
                        .Select(Notify.FromString)
                        .ToDictionary(x => x.NotifyId));
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
