using System.ComponentModel;

namespace Common.Enum
{
    public enum FrequencyType
    {
        [Description("Один раз")]
        Once = 0,
        [Description("Каждую минуту")]
        Minute = 1,
        [Description("Каждый час")]
        Hour = 2,
        [Description("Каждый день")]
        Day = 3,
        [Description("Каждую неделю")]
        Week = 4,
        [Description("Кастомно")]
        Custom = 99
    }
}
