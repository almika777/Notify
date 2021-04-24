using Microsoft.Extensions.Logging;
using Notify.Commands;
using Notify.IO;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Notify
{
    public class NotifyService
    {
        private readonly TelegramBotClient _bot;
        private readonly INotifyIOHandler _handler;
        private readonly NotifyCache _cache;
        private readonly CommandRepository _commandRepository;
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(TelegramBotClient bot, INotifyIOHandler handler, NotifyCache cache,
            CommandRepository commandRepository, ILogger<NotifyService> logger)
        {
            _bot = bot;
            _handler = handler;
            _cache = cache;
            _commandRepository = commandRepository;
            _logger = logger;
        }

        public void OnOnCallbackQuery(object? sender, CallbackQueryEventArgs e)
        {


        }

        public void OnMessage(object? sender, MessageEventArgs e)
        {
            if (_commandRepository.IsCommand(e.Message.Text))
            {
                _commandRepository.Execute(e.Message.Text, e);
                return;
            }

            try
            {
                var notifyModel = _cache.InProgressNotifications.TryGetValue(e.Message.Chat.Id, out var model)
                    ? model.Update(e.Message.Text)
                    : CreateNotify(e);

                _bot.SendTextMessageAsync(e.Message.Chat.Id, notifyModel.GetNextStepMessage());
                _handler.Write(notifyModel);
            }
            catch (FormatException exception)
            {
                _bot.SendTextMessageAsync(e.Message.Chat.Id, exception.Message);
            }
        }

        private NotifyModel CreateNotify(MessageEventArgs e)
        {
            var newModel = new NotifyModel { ChatId = e.Message.Chat.Id, NotifyId = Guid.NewGuid() }.Update(e.Message.Text);
            _cache.TryAdd(e.Message.Chat.Id, newModel);
            return newModel;
        }
    }
}
