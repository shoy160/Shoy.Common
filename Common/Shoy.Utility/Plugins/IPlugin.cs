using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoy.Utility.Plugins
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin
    {

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
