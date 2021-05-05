using Common.Common.Enum;
using System;

namespace Common.Common.CallbackModels
{
    public class CallbackFrequency
    {
        public static FrequencyType FromCallback(string data) =>
            (FrequencyType) (int.TryParse(data.Split(CommonResource.Separator)[1], out var frequency) 
                ? frequency 
                : throw new FormatException("Что-то с Frequncy в кнопках"));

        public static string ToCallBack(FrequencyType frequency) 
            => string.Join(CommonResource.Separator, "frequency", (int)frequency);
    }
}
