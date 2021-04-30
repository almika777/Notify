using Notify.Common;
using System.IO;

namespace TestsCommon
{
    public class TestBase
    {

        protected void Finish(Configuration config)
        {
            if (Directory.Exists(config.CacheFolder)) Directory.Delete(config.CacheFolder!, true);
        }
    }
}
