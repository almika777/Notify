using Microsoft.Extensions.Options;
using Notify.Common;

namespace CommonTests
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
