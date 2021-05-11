﻿using Common.Models;
using System.Threading.Tasks;
using Services.Cache;
using Services.IoServices;
using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyReadyStep : INotifyStep
    {
        private readonly NotifyCacheService _cache;
        private readonly INotifyWriter _writer;
        private readonly TelegramBotClient _bot;

        public NotifyReadyStep(TelegramBotClient bot, NotifyCacheService cache, INotifyWriter writer)
        {
            _cache = cache;
            _writer = writer;
            _bot = bot;
        }

        public async Task Execute(long chatId, Notify notifyModel)
        {
            _cache.TryRemoveFromCurrent(notifyModel);
            _cache.TryAddToMemory(notifyModel);
            await _writer.Write(notifyModel);
            await _bot.SendTextMessageAsync(chatId, notifyModel.GetNextStepMessage());
        }
    }
}
