using System;
using Notify.Common;

namespace CommonTests
{
    public class GlobalModel
    {
        public static NotifyModel GetModel()
        {
            return new NotifyModel
            {
                ChatId = 1,
                Date = new DateTimeOffset(new DateTime(2021, 01, 01)),
                Name = "model",
                NextStep = NotifyStep.Ready,
                NotifyId = Guid.NewGuid()
            };
        }

        public static NotifyModel GetModel(NotifyStep step)
        {
            return new NotifyModel
            {
                ChatId = 1,
                Date = new DateTimeOffset(new DateTime(2021, 01, 01)),
                Name = "model",
                NextStep = step,
                NotifyId = Guid.NewGuid()
            };
        }
    }
}
