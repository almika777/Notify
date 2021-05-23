using Common.Models;
using Microsoft.Extensions.Logging;
using Services.Cache;
using Services.IoServices;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyReadyStep : INotifyStep
    {
        private readonly NotifyCacheService _cache;
        private readonly INotifyWriter _writer;
        private readonly INotifyEditor _editor;
        private readonly TelegramBotClient _bot;
        private readonly ILogger<NotifyReadyStep> _logger;

        public NotifyReadyStep(TelegramBotClient bot, NotifyCacheService cache, INotifyWriter writer, INotifyEditor editor, ILoggerFactory loggerFactory)
        {
            _cache = cache;
            _writer = writer;
            _editor = editor;
            _bot = bot;
            _logger = loggerFactory.CreateLogger<NotifyReadyStep>();
        }

        public async Task Execute(long chatId, NotifyModel notifyModelModel)
        {
            try
            {
                _cache.TryAddToByUser(notifyModelModel);

                if (_cache.EditCache.TryGetValue(chatId, out var value))
                {
                    if (value.NotifyId == notifyModelModel.NotifyId)
                    {
                        await _editor.Edit(chatId, notifyModelModel);
                        _cache.EditCache.TryRemove(chatId, out _);
                    }
                }
                else
                {
                    await _writer.Write(notifyModelModel);
                }

                await _bot.SendTextMessageAsync(chatId, notifyModelModel.GetNextStepMessage());
                _cache.TryRemoveFromCurrent(notifyModelModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Беда");
            }
        }
    }
}
