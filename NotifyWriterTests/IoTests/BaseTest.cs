using Common.Enum;
using Common.Models;
using System;
using System.Collections.Generic;

namespace NotifyTests.IoTests
{
    public class BaseTest
    {
        protected IEnumerable<NotifyModel> GetDefaultManyModels(long userId = 1)
        {
            var rand = new Random().Next(3, 10);
            var result = new List<NotifyModel> { GetDefaultModel(userId) };

            for (int i = 0; i < rand; i++)
            {
                result.Add(new NotifyModel
                {
                    Date = DateTimeOffset.Now,
                    Name = "test" + i,
                    Frequency = FrequencyType.Once,
                    FrequencyTime = TimeSpan.FromMinutes(1),
                    NotifyId = Guid.NewGuid(),
                    UserId = userId
                });
            }

            return result;
        }

        protected NotifyModel GetDefaultModel(long userId = 1)
        {
            return new NotifyModel
            {
                Date = DateTimeOffset.Now,
                Name = "test",
                Frequency = FrequencyType.Minute,
                ChatUserModel = new ChatUserModel() { ChatId = userId },
                FrequencyTime = TimeSpan.FromMinutes(1),
                NotifyId = Guid.NewGuid(),
                UserId = userId
            };
        }
    }
}
