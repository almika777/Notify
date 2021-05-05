using Common.Common.CallbackModels;
using Common.Common.Enum;
using Services.Services;
using System.Threading.Tasks;
using Services.Helpers;
using Services.Helpers.NotifyStepHandlers;
using Telegram.Bot;
using Telegram.Bot.Args;
// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery
{
    public class SetFrequencyOnCallback : ICallbackCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyModifier _modifier;
        private readonly NotifyStepHandlers _stepHandlers;
        private readonly NotifyCacheService _cache;

        public SetFrequencyOnCallback(NotifyCacheService cache, TelegramBotClient bot, NotifyModifier modifier, NotifyStepHandlers stepHandlers)
        {
            _bot = bot;
            _modifier = modifier;
            _stepHandlers = stepHandlers;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var chatId = e.CallbackQuery.Message.Chat.Id;
            if (!_cache.InProgressNotifications.TryGetValue(chatId, out var model)) return;

            if (model!.NextStep != NotifyStep.Frequency)
            {
                await _bot.SendTextMessageAsync(chatId, "");
                return;
            }

            model.Frequency = CallbackFrequency.FromCallback(e.CallbackQuery.Data);
            _modifier.CreateOrUpdate(model, $"{(int)model.Frequency}");
            await _stepHandlers.ReadyStep.Execute(chatId, model);
        }
    }
}
