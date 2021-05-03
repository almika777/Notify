using Common.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.Services.IoServices.FileServices
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

        public async Task<IEnumerable<NotifyModel>> ReadAll(long chatId)
        {
            var models = new List<NotifyModel>();

            using var sr = GetStreamReader(chatId);

            string currentLine;
            while ((currentLine = await sr.ReadLineAsync()) != null)
            {
                models.Add(NotifyModel.FromString(currentLine));
            }

            return models;
        }

        public async Task<NotifyModel> Read(long chatId, Guid notifyId)
        {
            try
            {
                using var sr = GetStreamReader(chatId);

                string ln;
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

        private StreamReader GetStreamReader(long chatId)
        {
            if (string.IsNullOrEmpty(_config.CacheFolder)) throw new ApplicationException("Не указан кжшфолдер. Ну епрст...");
            var path = Path.Combine(_config.CacheFolder!, $@"{chatId}.txt");

            return new StreamReader(path);
        }
    }
}
