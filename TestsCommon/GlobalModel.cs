using System;
using Common.Enum;

namespace CommonTests
{
    public class GlobalModel
    {
        public static Common.Models.NotifyModel GetModel()
        {
            return new Common.Models.NotifyModel
            {
                UserId = 1,
                Date = new DateTimeOffset(new DateTime(2021, 01, 01)),
                Name = "model",
                NextStep = NotifyStep.Ready,
                NotifyId = Guid.NewGuid()
            };
        }

        public static Common.Models.NotifyModel GetModel(NotifyStep step)
        {
            return new Common.Models.NotifyModel
            {
                UserId = 1,
                Date = new DateTimeOffset(new DateTime(2021, 01, 01)),
                Name = "model",
                NextStep = step,
                NotifyId = Guid.NewGuid()
            };
        }
    }
}
