using System.Threading.Tasks;
using Common.Common;
using Services.Services;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Services.Commands.OnCallbackQuery
{
    public class RemoveOnCallbackCommand : ICallbackCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public RemoveOnCallbackCommand(NotifyCacheService cache, TelegramBotClient bot)
        {
            _bot = bot;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var key = e.CallbackQuery.Message.Chat.Id;
            var callbackData = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
            _cache.ByUser.TryGetValue(key, out var models);

            if (models!.ContainsKey(callbackData.NotifyId)) models.Remove(callbackData.NotifyId);

            await _bot.DeleteMessageAsync(key, e.CallbackQuery.Message.MessageId);
        }
    }
}
