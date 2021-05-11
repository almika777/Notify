﻿using Common.Models;
using Services.Cache;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Services.Helpers.NotifyStepHandlers
{
    public class NotifyDateStep : INotifyStep
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public NotifyDateStep(TelegramBotClient bot, NotifyCacheService cache)
        {
            _bot = bot;
            _cache = cache;
        }

        public async Task Execute(long chatId, Notify notifyModel)
        {
            _cache.InProgressNotifications.TryAdd(chatId, notifyModel);
            await _bot.SendTextMessageAsync(chatId, notifyModel.GetNextStepMessage());
        }
    }
}
