using Shoy.Core.Modules;
using Shoy.Utility.Logging;

namespace Shoy.MongoDb
{
    /// <summary> MongoDB模块 </summary>
    public class MongoDbModule : DModule
    {
        private readonly ILogger _logger = LogManager.Logger<MongoDbModule>();
        public override void Initialize()
        {
            base.Initialize();
            _logger.Debug("MongoDb Initialize...");
        }
    }
}
