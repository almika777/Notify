using System.IO;
using Common;

namespace CommonTests
{
    public class TestBase
    {
        protected void Finish(Configuration config)
        {
            if (Directory.Exists(config.CacheFolder)) Directory.Delete(config.CacheFolder!, true);
        }
    }
}
