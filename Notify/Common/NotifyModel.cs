using System;

namespace Notify.Common
{

    public class NotifyModel
    {
        public long ChatId { get; set; }
        public Guid NotifyId { get; set; }
        public DateTimeOffset Date { get; set; }
        public NotifyState State { get; set; }
        public string Name { get; set; } = null!;

        public override string ToString() => string.Join('|', Name, Date);

        public void UpdateState()
        {
            State = State switch
            {
                NotifyState.Name => NotifyState.Date,
                NotifyState.Date => NotifyState.Ready,
                _ => State
            };
        }
    }
}
