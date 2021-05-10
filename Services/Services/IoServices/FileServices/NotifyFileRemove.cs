using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Models;

namespace Services.Services.IoServices.FileServices
{
    public class NotifyFileRemove : INotifyRemover
    {
        private readonly Configuration _config;
        private readonly INotifyReader _reader;
        private readonly INotifyWriter _writer;
        private readonly ILogger<NotifyFileRemove> _logger;

        public NotifyFileRemove(
            INotifyReader reader, 
            INotifyWriter writer,
            IOptions<Configuration> config, 
            ILogger<NotifyFileRemove> logger)
        {
            _config = config.Value;
            _reader = reader;
            _writer = writer;
            _logger = logger;
        }

        public async Task<bool> Remove(Notify model)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) return false;

            var path = Path.Combine(_config.CacheFolder!, $@"{model.UserId}.txt");

            if (!File.Exists(path)) throw new FileNotFoundException("Не нашли файлик");

            try
            {
                var models = (await _reader.ReadAll(model.UserId)).ToImmutableArray();
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
