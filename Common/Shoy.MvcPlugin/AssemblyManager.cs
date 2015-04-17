using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Shoy.MvcPlugin
{
    /// <summary>
    /// 程序集管理
    /// </summary>
    public class AssemblyManager
    {
        /// <summary>
        /// 获取程序集最后写入时间
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static long GetAssemblyTicks(Assembly assembly)
        {
            return GetAssemblyTime(assembly).Ticks;
        }
        /// <summary>
        /// 获取程序集最后写入时间
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static DateTime GetAssemblyTime(Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            return File.GetLastWriteTime(new Uri(assemblyName.CodeBase).LocalPath);
        }
        /// <summary>
        /// 获取文件写入毫秒数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static long GetFileWriteTicks(string path)
        {
            return File.GetLastWriteTime(path).Ticks;
        }

        /// <summary>
        /// 获取程序集属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(ICustomAttributeProvider assembly, bool inherit = false)
            where T : Attribute
        {
            return assembly
                .GetCustomAttributes(typeof(T), inherit)
                .OfType<T>()
                .FirstOrDefault();
        }

        /// <summary>
        /// 获取当前程序集信息
        /// </summary>
        /// <param name="pluAssembly"></param>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public static PluginAssembly GetPlusAssemblysInfo(Assembly pluAssembly, FileInfo dllFile)
        {
            var plus = pluAssembly.GetName();
            var descriptionAttr = GetAttribute<AssemblyDescriptionAttribute>(pluAssembly);
            var titleAttr = GetAttribute<AssemblyTitleAttribute>(pluAssembly);
            var guidAttr = GetAttribute<GuidAttribute>(pluAssembly);
            var plusAss = new PluginAssembly
            {
                PluginKey = (guidAttr == null ? Guid.NewGuid() : Guid.Parse(guidAttr.Value)),
                Description = descriptionAttr == null ? string.Empty : descriptionAttr.Description,
                PluginName = plus.Name,
                Title = titleAttr == null ? string.Empty : titleAttr.Title,
                UpdateTime = GetAssemblyTime(pluAssembly),
                Version = plus.Version
            };
            //config
            return plusAss;
        }
    }
}
