using Microsoft.Extensions.Logging;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify
{
    public class NotifyService
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyCache _cache;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(TelegramBotClient bot, NotifyCache cache, ILogger<NotifyService> logger)
        {
            _bot = bot;
            _cache = cache;
            _logger = logger;
        }


        public void OnMessage(object? sender, MessageEventArgs e)
        {
            if (e.Message.Text.Equals("/start")) ReplyToStart(e.Message.Chat.Id);

            if (_cache.ByUser.TryGetValue(e.Message.Chat.Id, out var model))
            {
                UpdateNotify(model, e);
                return;
            }

            CreateNotify(e);
        }

        private void UpdateNotify(NotifyModel model, MessageEventArgs e)
        {
            if (model.State == NotifyState.Ready) return;
            try
            {
                model.Update(e.Message.Text);
                if (model.State == NotifyState.Date) _cache.ByDate.TryAdd(model.Date, model);
                _bot.SendTextMessageAsync(e.Message.Chat.Id, model.GetNextStepMessage());
            }
            catch (Exception exception)
            {
                _bot.SendTextMessageAsync(e.Message.Chat.Id, exception.Message);
            }
        }

        private void CreateNotify(MessageEventArgs e)
        {
            var newModel = new NotifyModel { ChatId = e.Message.Chat.Id }.Update(e.Message.Text);
            _bot.SendTextMessageAsync(e.Message.Chat.Id, newModel.GetNextStepMessage());
            _cache.ByUser.TryAdd(e.Message.Chat.Id, newModel);
        }

        private void ReplyToStart(long chatId)
        {
            _bot.SendTextMessageAsync(chatId,
                $@"Привет, если ты вечно что-то забываешь, то я помогу тебе. 
                {Environment.NewLine}Хочешь добавить напоминалку просто жми");
        }
    }
}
