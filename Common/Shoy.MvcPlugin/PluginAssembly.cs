using System;

namespace Shoy.MvcPlugin
{
    /// <summary> 插件描述类 </summary>
    public class PluginAssembly : IComparable<PluginAssembly>
    {
        /// <summary> 插件唯一主键 </summary>
        public virtual Guid PluginKey { get; set; }

        public string Key { get { return PluginKey.ToString("N"); } }

        /// <summary> 插件名称 </summary>
        public virtual string PluginName { get; set; }

        public virtual string FullName { get; set; }

        /// <summary> 插件版本 </summary>
        public virtual Version Version { get; set; }

        /// <summary> 更新时间 </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary> 插件标题 </summary>
        public virtual string Title { get; set; }
        /// <summary> 插件描述 </summary>
        public virtual string Description { get; set; }

        /// <summary> 作者 </summary>
        public virtual string Author { get; set; }

        /// <summary> 插件加载顺序 </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary> 是否已安装 </summary>
        public virtual bool Installed { get; set; }

        /// <summary>
        /// 插件比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PluginAssembly other)
        {
            return Version.CompareTo(other.Version);
        }
    }
}
