using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Notify.IO.File
{
    public class NotifyFileReader : INotifyReader
    {
        private readonly ILogger<NotifyFileReader> _logger;
        private readonly Configuration _config;

        public NotifyFileReader(IOptions<Configuration> config, ILogger<NotifyFileReader> logger)
        {
            _logger = logger;
            _config = config.Value;
        }
        public Task<IEnumerable<NotifyModel>> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public async Task<NotifyModel> Read(long chatId, Guid notifyId)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) return new NotifyModel();

            var path = Path.Combine(_config.CacheFolder!, $@"{chatId}.txt");

            try
            {
                if (!Directory.Exists(_config.CacheFolder)) Directory.CreateDirectory(_config.CacheFolder);
                if (!System.IO.File.Exists(path)) await System.IO.File.Create(path).DisposeAsync();

                await using var fs = new FileStream(path, FileMode.Open);
                using var sr = new StreamReader(fs);

                string? ln;

                while ((ln = await sr.ReadLineAsync()) != null)
                {
                    var model = NotifyModel.FromString(ln);
                    if (model.NotifyId == notifyId) return model;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ой ей ей");
                return new NotifyModel();
            }

            return new NotifyModel();
        }
    }
}
