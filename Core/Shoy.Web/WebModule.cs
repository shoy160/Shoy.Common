using System;
using Shoy.Core.Modules;
using Shoy.Utility.Logging;

namespace Shoy.Web
{
    public class WebModule : DModule
    {
        private readonly ILogger _logger = LogManager.Logger<WebModule>();
        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("WebModule Initialize...");
        }
    }
}
