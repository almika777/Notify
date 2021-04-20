using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notify
{
    public class Startup : BackgroundService
    {
        private readonly ILogger<Startup> _logger;
        private readonly TelegramBotClient _bot = new TelegramBotClient(Resource.TelegramToken);
        private IDictionary<string, string> _commands = null!;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bot.OnMessage += BotOnOnMessage;
            _bot.OnCallbackQuery += BotOnOnCallbackQuery; // при нажатии на Inline кнопку
            _commands = (await _bot.GetMyCommandsAsync(stoppingToken))
                .ToDictionary(x => x.Command, x => x.Description);

            _bot.StartReceiving();
        }

        private void BotOnOnCallbackQuery(object? sender, CallbackQueryEventArgs e)
        {


        }


        private void BotOnOnMessage(object? sender, MessageEventArgs e)
        {
            if(e.Message.Text.Equals("/start")) ReplyToStart(e);
            if (_commands.ContainsKey(e.Message.Text.Substring(1)))
            {
                _bot.SendTextMessageAsync(e.Message.Chat.Id, "Set", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                {
                    CallbackData = "qwe",
                    Text = "qwe"
                }));
            }
        }

        private void ReplyToStart(MessageEventArgs e)
        {
            _bot.SendTextMessageAsync(e.Message.Chat.Id, $@"
                Привет, если ты вечно что-то забываешь, то я помогу тебе. {Environment.NewLine}{Environment.NewLine}Хочешь добавить напоминалку просто жми",
                replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                {
                    CallbackData = "Добавить",
                    Text = "Добавить"
                }));
        }
    }
}
