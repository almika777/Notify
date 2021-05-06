using System;
using System.ComponentModel;
using System.Reflection;

namespace Common.Extensions
{
    public static class FrequencyTypeExtensions
    {
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
            }

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString()!);
            if (memberInfo.Length <= 0) return enumerationValue.ToString()!;
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attrs.Length > 0 
                ? ((DescriptionAttribute)attrs[0]).Description 
                : enumerationValue.ToString())!;
        }
    }
}
