using Common;
using Common.Common;
using Microsoft.Extensions.Logging;
using Services.Commands.OnCallbackQuery.Edit;
using Services.Commands.OnMessage;
using Services.Services;
using Services.Services.IoServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnCallbackQuery
{
    public class OnCallbackCommandRepository
    {
        public IDictionary<string, ICallbackCommand> OnCallbackCommands { get; } = new Dictionary<string, ICallbackCommand>();
        private readonly NotifyCacheService _cache;
        private readonly ILogger<OnCallbackCommandRepository> _logger;

        public OnCallbackCommandRepository(
            OnMessageCommandRepository messageCommandRepository,
            NotifyCacheService cache,
            TelegramBotClient bot,
            INotifyRemover fileRemover,
            ILogger<OnCallbackCommandRepository> logger)
        {
            _cache = cache;
            _logger = logger;
            Init(messageCommandRepository, cache, bot, fileRemover);
        }

        public Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            try
            {
                var callbackDataParams = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
                var command = callbackDataParams.Command.ToLower().Trim().Split(CommonResource.Separator);
                return OnCallbackCommands[command[0]].Execute(sender, e);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Упс");
                return Task.CompletedTask;
            }
        }

        private void Init(OnMessageCommandRepository messageCommandRepository, NotifyCacheService cache, TelegramBotClient bot,
            INotifyRemover fileRemover)
        {
            OnCallbackCommands.Add(BotCommands.OnCallback.Show, new ShowOnCallback(cache, bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.Remove, new RemoveOnCallback(messageCommandRepository, fileRemover, cache, bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.EditEntry, new EditOnCallback(messageCommandRepository, fileRemover, cache, bot));
            OnCallbackCommands.Add(BotCommands.OnCallback.SetFrequency, new SetFrequencyOnCallback(messageCommandRepository, fileRemover, cache, bot));
            InitEditors(cache, bot);
        }

        private void InitEditors(NotifyCacheService cache, TelegramBotClient bot)
        {
            var editor = new EditNotifyOnCallback(cache, bot);

            OnCallbackCommands.Add(BotCommands.OnCallback.EditNotifyName, editor);
            OnCallbackCommands.Add(BotCommands.OnCallback.EditNotifyDate, editor);
        }
    }
}
