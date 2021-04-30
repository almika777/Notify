﻿using Common.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable 8620

namespace Services.Services.IoServices.FileServices
{
    public class NotifyFileWriterService : INotifyWriter
    {
        private readonly ILogger<NotifyFileWriterService> _logger;
        private readonly Configuration _config;

        public NotifyFileWriterService(IOptions<Configuration> config, ILogger<NotifyFileWriterService> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task Write(NotifyModel model)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) return;

            var path = Path.Combine(_config.CacheFolder!, $@"{model.ChatId}.txt");

            try
            {
                if (!Directory.Exists(_config.CacheFolder)) Directory.CreateDirectory(_config.CacheFolder);
                if (!File.Exists(path)) await File.Create(path).DisposeAsync();

                await File.AppendAllLinesAsync(path, new[] { model.ToString() });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ой ей ей");
            }
        }

        public async Task Write(IEnumerable<NotifyModel> models)
        {
            var notifyModels = models.ToArray();

            if (string.IsNullOrEmpty(_config.CacheFolder) || !notifyModels.Any()) return;

            var chatId = notifyModels.First().ChatId;
            var path = Path.Combine(_config.CacheFolder!, $@"{chatId}.txt");

            try
            {
                if (!Directory.Exists(_config.CacheFolder)) Directory.CreateDirectory(_config.CacheFolder);
                if (!File.Exists(path)) await File.Create(path).DisposeAsync();

                await File.AppendAllLinesAsync(path, notifyModels.Select(x => x.ToString()));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ой ей ей");
            }
        }
    }
}

