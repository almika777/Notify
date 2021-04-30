﻿using Common.Common;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Services.Services.IoServices.FileServices
{
    public class NotifyFileRemoveService : INotifyRemover
    {
        private readonly Configuration _config;
        private readonly INotifyReader _reader;
        private readonly INotifyWriter _writer;
        private readonly ILogger<NotifyFileRemoveService> _logger;

        public NotifyFileRemoveService(INotifyReader reader, INotifyWriter writer, IOptions<Configuration> config, ILogger<NotifyFileRemoveService> logger)
        {
            _config = config.Value;
            _reader = reader;
            _writer = writer;
            _logger = logger;
        }

        public async Task<bool> Remove(NotifyModel model)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) return false;

            var path = Path.Combine(_config.CacheFolder!, $@"{model.ChatId}.txt");

            if (!File.Exists(path)) throw new FileNotFoundException("Не нашли файлик");

            try
            {
                var models = (await _reader.ReadAll(model.ChatId)).ToImmutableArray();
                File.Delete(path);

                await _writer.Write(models.Where(x => !x.Equals(model)));
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ой ей ей");
                return false;
            }
        }
    }
}
