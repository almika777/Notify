﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Notify.IO
{
    public class NotifyFileWriter : INotifyWriter
    {
        private readonly ILogger<NotifyFileWriter> _logger;
        private readonly Configuration _config;

        public NotifyFileWriter(IOptions<Configuration> config, ILogger<NotifyFileWriter> logger)
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

                await File.AppendAllLinesAsync(path, new []{model.ToString()});
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ой ей ей");
            }
        }
    }
}
