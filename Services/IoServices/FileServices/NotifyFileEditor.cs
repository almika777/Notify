using Common;
using Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.IoServices.FileServices
{
    public class NotifyFileEditor : INotifyEditor
    {
        private readonly INotifyReader _reader;
        private readonly INotifyWriter _writer;
        private readonly ILogger<NotifyFileEditor> _logger;
        private readonly Configuration _config;

        public NotifyFileEditor(
            INotifyReader reader, 
            INotifyWriter writer, 
            IOptions<Configuration> config, 
            ILogger<NotifyFileEditor> logger)
        {
            _reader = reader;
            _writer = writer;
            _logger = logger;
            _config = config.Value;
        }

        public async Task<bool> Edit(long chatId, Notify model)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) return false;

            var path = Path.Combine(_config.CacheFolder!, $@"{chatId}.txt");

            if (!File.Exists(path)) throw new FileNotFoundException("Не нашли файлик");

            try
            {
                var models = (await _reader.ReadAll(chatId)).ToList();
                File.Delete(path);

                var index = models.IndexOf(model);
                models[index] = model;
                await _writer.Write(models);
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
