using Microsoft.Extensions.Options;
using Notify.Common;

namespace TestsCommon
{
    public class GlobalConfig : Configuration
    {
        public static IOptions<Configuration> GetDefault()
        {
            return new OptionsWrapper<Configuration>(new Configuration
            {
                CacheFolder = "../../tests"
            });
        }
    }
}
