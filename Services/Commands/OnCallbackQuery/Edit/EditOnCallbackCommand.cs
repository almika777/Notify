﻿using System;
using System.Threading.Tasks;
using Common;
using Common.Common;
using Services.Commands.OnMessage;
using Services.Services;
using Services.Services.IoServices;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

// ReSharper disable PossibleNullReferenceException

namespace Services.Commands.OnCallbackQuery.Edit
{
    public class EditOnCallbackCommand : ICallbackCommand
    {
        private readonly OnMessageCommandRepository _messageCommandRepository;
        private readonly INotifyRemover _notifyFileRemover;
        private readonly TelegramBotClient _bot;
        private readonly NotifyCacheService _cache;

        public EditOnCallbackCommand(
            OnMessageCommandRepository messageCommandRepository,
            INotifyRemover notifyFileRemover,
            NotifyCacheService cache,
            TelegramBotClient bot)
        {
            _messageCommandRepository = messageCommandRepository;
            _notifyFileRemover = notifyFileRemover;
            _bot = bot;
            _cache = cache;
        }

        public async Task Execute(object? sender, CallbackQueryEventArgs e)
        {
            var chatId = e.CallbackQuery.Message.Chat.Id;
            _cache.ByUser.TryGetValue(chatId, out var models);

            var callbackData = CallbackDataModel.FromCallbackData(e.CallbackQuery.Data);
            models.TryGetValue(callbackData.NotifyId, out var model);

            var markup = new InlineKeyboardMarkup(new[]{
                new InlineKeyboardButton
                {
                    Text = "Название",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.EditNameCallbackCommand, model!.NotifyId),
                },
                new InlineKeyboardButton
                {
                    Text = "Дату",
                    CallbackData = CallbackDataModel.ToCallbackData(BotCommands.OnCallback.EditDateCallbackCommand, model!.NotifyId),
                }
            });

            await _bot.DeleteMessageAsync(chatId, e.CallbackQuery.Message.MessageId);
            await _bot.SendTextMessageAsync(chatId, "Что меняем?", replyMarkup: markup);
        }
    }
}