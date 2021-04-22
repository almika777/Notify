using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Notify.IO
{
    public class NotifyFileHandler : INotifyIOHandler
    {
        private const string PartOfPath = "../../";
        public async Task Write(NotifyModel model)
        {
            await using var sw = new StreamWriter(Path.Combine(PartOfPath, $@"{model.ChatId}.txt"));
            await sw.WriteLineAsync(model.ToString());
        }

        public async Task<IEnumerable<NotifyModel>> Read(long chatId)
        {
            using var sr = new StreamReader(Path.Combine(PartOfPath, $@"{chatId}.txt"));

            var notifications = new List<NotifyModel>();

            string line;
            while ((line = (await sr.ReadLineAsync())!) != null)
            {
                notifications.Add(ModelFromString(line));
            }

            return notifications;
        }

        public Task<NotifyModel> Read(Guid notifyId)
        {
            throw new NotImplementedException();
        }

        private NotifyModel ModelFromString(string modelString)
        {
            var properties = modelString.Split('|');

            return new NotifyModel
            {
                Name = properties[0],
                Date = DateTimeOffset.TryParse(properties[1], out var date)
                    ? date
                    : throw new FormatException("Мы как-то записали неверный формат даты")
            };
        }
    }
}

