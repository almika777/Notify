using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notify
{
    public class Startup : BackgroundService
    {
        private readonly NotifyCache _cache;
        private readonly ILogger<Startup> _logger;
        private readonly TelegramBotClient _bot = new TelegramBotClient(Resource.TelegramToken);

        public Startup(NotifyCache cache, ILogger<Startup> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bot.OnMessage += BotOnOnMessage;
            _bot.OnCallbackQuery += BotOnOnCallbackQuery;

            _bot.StartReceiving();
        }

        private void BotOnOnCallbackQuery(object? sender, CallbackQueryEventArgs e)
        {


        }


        private void BotOnOnMessage(object? sender, MessageEventArgs e)
        {
            if (e.Message.Text.Equals("/start")) ReplyToStart(e);

            if (_cache.UserCurrentNotify.TryGetValue(e.Message.Chat.Id, out var model))
            {
                _bot.SendTextMessageAsync(e.Message.Chat.Id, model.GetNextStepMessage());
                model.Update(e.Message.Text);
                return;
            }

            var newModel = new NotifyModel().Update(e.Message.Text);
            _bot.SendTextMessageAsync(e.Message.Chat.Id, newModel.GetNextStepMessage());
            _cache.UserCurrentNotify.TryAdd(e.Message.Chat.Id, newModel);
        }

        private void ReplyToStart(MessageEventArgs e)
        {
            _bot.SendTextMessageAsync(e.Message.Chat.Id, 
                $@"Привет, если ты вечно что-то забываешь, то я помогу тебе. 
                {Environment.NewLine}Хочешь добавить напоминалку просто жми");
        }
    }
}
