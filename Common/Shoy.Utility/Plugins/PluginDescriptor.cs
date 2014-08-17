using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Shoy.Utility.Plugins
{
    public class PluginDescriptor : IComparable<PluginDescriptor>
    {
        /// <summary>
        /// 插件文件名
        /// </summary>
        public virtual string PluginFileName { get; set; }

        /// <summary>
        /// 插件类型
        /// </summary>
        public virtual Type PluginType { get; set; }

        public virtual Assembly ReferencedAssembly { get; internal set; }
        public virtual FileInfo OriginalAssemblyFile { get; internal set; }

        /// <summary>
        /// 分组
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        /// 友好的名称
        /// </summary>
        public virtual string FriendlyName { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// 被支持的版本列表
        /// </summary>
        public virtual IList<string> SupportedVersions { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        public virtual IList<int> LimitedToStores { get; set; }

        /// <summary>
        /// 是否已安装
        /// </summary>
        public virtual bool Installed { get; set; }

        public PluginDescriptor()
        {
            SupportedVersions = new List<string>();
            LimitedToStores = new List<int>();
        }


        public PluginDescriptor(Assembly referencedAssembly, FileInfo originalAssemblyFile,
            Type pluginType)
            : this()
        {
            ReferencedAssembly = referencedAssembly;
            OriginalAssemblyFile = originalAssemblyFile;
            PluginType = pluginType;
        }

        //public virtual T Instance<T>() where T : class, IPlugin
        //{
        //    object instance;
        //    if (!EngineContext.Current.ContainerManager.TryResolve(PluginType, null, out instance))
        //    {
        //        //not resolved
        //        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(PluginType);
        //    }
        //    var typedInstance = instance as T;
        //    if (typedInstance != null)
        //        typedInstance.PluginDescriptor = this;
        //    return typedInstance;
        //}

        //public IPlugin Instance()
        //{
        //    return Instance<IPlugin>();
        //}

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PluginDescriptor other)
        {
            if (DisplayOrder != other.DisplayOrder)
                return DisplayOrder.CompareTo(other.DisplayOrder);
            return String.Compare(FriendlyName, other.FriendlyName, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PluginDescriptor;
            return other != null &&
                   SystemName != null &&
                   SystemName.Equals(other.SystemName);
        }

        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }
    }
}
