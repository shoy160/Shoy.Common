
namespace Shoy.Utility.Plugins
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin
    {
        PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// 安装插件
        /// </summary>
        void Install();
        /// <summary>
        /// 卸载插件
        /// </summary>
        void Uninstall();
    }
}
