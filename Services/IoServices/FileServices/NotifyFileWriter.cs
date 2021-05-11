using Common;
using Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable 8620

namespace Services.IoServices.FileServices
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

        public async Task Write(Notify model)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) return;

            var path = Path.Combine(_config.CacheFolder!, $@"{model.UserId}.txt");

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

        public async Task Write(IEnumerable<Notify> models)
        {
            var notifyModels = models.ToArray();

            if (string.IsNullOrEmpty(_config.CacheFolder) || !notifyModels.Any()) return;

            var chatId = notifyModels.First().UserId;
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

