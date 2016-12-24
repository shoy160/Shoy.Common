using Shoy.Framework.Logging;
using Shoy.Utility.Logging;

namespace Shoy.Common.Test
{
    public abstract class TestBase
    {
        public TestBase()
        {
            LogManager.AddAdapter(new Log4NetAdapter());
        }
    }
}
