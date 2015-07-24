
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shoy.MvcPlugin
{
    /// <summary> 插件管理基类 </summary>
    public abstract class PluginManagerBase
    {
        public DirectoryInfo DynamicDirectory = new DirectoryInfo(AppDomain.CurrentDomain.DynamicDirectory);

        public string PluginXmlPath;

        /// <summary> 已安装插件列表 </summary>
        public abstract List<PluginAssembly> PluginsList { get; set; }

        /// <summary> 部署程序集 </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public abstract Assembly Deploy(FileInfo dllFile);

        /// <summary>
        /// 获取插件信息与配置
        /// </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public abstract PluginAssembly GetPluginAssembly(FileInfo dllFile);

        /// <summary>
        /// 获取插件信息与配置
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public abstract PluginAssembly GetPluginAssembly(Assembly assembly, FileInfo dllFile);

        /// <summary>
        /// 获取插件信息与配置
        /// </summary>
        /// <returns></returns>
        public virtual PluginAssembly GetPluginAssembly(string fullName)
        {
            return PluginsList.FirstOrDefault(p => p.FullName.Equals(fullName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// 判断插件是否已经安装
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns> </returns>
        public virtual bool IsInstalled(string fullName)
        {
            return
                PluginsList.FirstOrDefault(
                    p => p.Installed && p.FullName.Equals(fullName, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        /// <summary>
        /// 加载策略
        /// </summary>
        /// <param name="assembly">程序集</param>
        public abstract void LoadPlusStrategys(Assembly assembly);

        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="assembly"></param>
        public abstract void Install(Assembly assembly);

        /// <summary>
        /// 卸载插件
        /// </summary>
        public abstract void Uninstall(Assembly assembly);

        /// <summary>
        /// 将dll复制到动态程序集目录
        /// </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public abstract FileInfo CopyToDynamicDirectory(FileInfo dllFile);
    }
}
