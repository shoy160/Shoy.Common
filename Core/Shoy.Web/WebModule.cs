using System;
using Shoy.Core.Modules;

namespace Shoy.Web
{
    public class WebModule : DModule
    {
        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("WebModule Initialize...");
        }
    }
}
