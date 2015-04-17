
namespace Shoy.MvcPlugin
{
    /// <summary> 插件接口 </summary>
    public interface IPlugin
    {
        /// <summary> 初始化插件 </summary>
        void Initialize();

        /// <summary> 安装插件 </summary>
        void Install();

        /// <summary> 卸载插件 </summary>
        void Uninstall();
    }
}
