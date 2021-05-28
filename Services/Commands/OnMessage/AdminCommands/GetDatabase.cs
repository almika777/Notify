using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;

namespace Services.Commands.OnMessage.AdminCommands
{
    public class GetDatabase : IAdminMessageCommand
    {
        private readonly TelegramBotClient _bot;
        private readonly Configuration _configuration;
        private readonly ILogger<GetDatabase> _logger;

        public GetDatabase(TelegramBotClient bot, Configuration configuration, ILoggerFactory factory)
        {
            _bot = bot;
            _configuration = configuration;
            _logger = factory.CreateLogger<GetDatabase>();
        }


        public async Task Execute(MessageEventArgs args)
        {
            var chatId = args.Message.Chat.Id;
            try
            {
                var zipFile = @"../../db.zip";
                CreateZip(zipFile);

                await using (var stream = File.OpenRead(zipFile))
                {
                    await _bot.SendDocumentAsync(chatId, new InputOnlineFile(stream), "db.zip");
                }

                File.Delete(zipFile);
            }
            catch (Exception e)
            {
                await _bot.SendTextMessageAsync(chatId, e.Message);
                _logger.LogError(e, "Get db error");
            }
        }

        private void CreateZip(string zipFile)
        {
            var archiveMode = File.Exists(zipFile) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
            var archive = ZipFile.Open(zipFile, archiveMode);

            archive.CreateEntryFromFile(_configuration.DbPath, Path.GetFileName(_configuration.DbPath));
            archive.Dispose();
        }
    }
}
