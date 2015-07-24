using System.Reflection;

namespace Shoy.MvcPlugin
{
    public class PluginInfo
    {
        /// <summary> 程序集 </summary>
        public virtual Assembly Assembly { get; set; }
        /// <summary> 插件程序集 </summary>
        public virtual PluginAssembly PluginAssembly { get; set; }
    }
}
