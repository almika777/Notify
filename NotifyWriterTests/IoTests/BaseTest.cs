using Common.Enum;
using Common.Models;
using System;

namespace NotifyTests.IoTests
{
    public class BaseTest
    {
        protected NotifyModel GetDefaultModel()
        {
            return  new NotifyModel
            {
                Date = DateTimeOffset.Now,
                Name = "test",
                Frequency = FrequencyType.Minute,
                ChatUserModel = new ChatUserModel { ChatId = 1 },
                FrequencyTime = TimeSpan.FromMinutes(1),
                NotifyId = Guid.NewGuid()
            };
        }
    }
}
