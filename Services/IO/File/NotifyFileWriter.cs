using System;
using System.IO;
using System.Threading.Tasks;
using Common.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable 8620

namespace Services.IO.File
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
                if (!System.IO.File.Exists(path)) await System.IO.File.Create(path).DisposeAsync();

                await System.IO.File.AppendAllLinesAsync(path, new[] { model.ToString() });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ой ей ей");
            }
        }
    }
}

