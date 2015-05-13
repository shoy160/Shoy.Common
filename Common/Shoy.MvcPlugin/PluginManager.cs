using System.Web;
using Shoy.MvcPlugin;
using Shoy.Utility;
using Shoy.Utility.Extend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Compilation;
using Shoy.Utility.Helper;

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
namespace Shoy.MvcPlugin
{
    public class PluginManager : PluginManagerBase
    {
        public PluginManager()
        {
            _pluginsList = (File.Exists(PluginXmlPath)
                ? XmlHelper.XmlDeserialize<List<PluginAssembly>>(PluginXmlPath)
                : new List<PluginAssembly>());
        }

        private const string PluginPrefix = "Shoy.Plugin.";

        /// <summary>
        /// 初始化加载
        /// </summary>
        public static void Initialize()
        {
            var plusFilesDirectoryInfo = new DirectoryInfo(Utils.GetMapPath("~/App_Data/Plugins"));

            //插件前缀必须为Magicodes
            var plusDlls = plusFilesDirectoryInfo.GetFiles("*.dll", SearchOption.AllDirectories).ToList();
            if (plusDlls.Count == 0) return;

            var manager = new PluginManager();

            if (!AppDomain.CurrentDomain.IsFullyTrusted)
                throw new Exception("请将当前应用程序信任级别设置为完全信任");
            var otherDlls =
                plusDlls.Where(t => t.Name.StartsWith(PluginPrefix, StringComparison.InvariantCultureIgnoreCase));
            var binDir = new DirectoryInfo(Utils.GetMapPath("~/bin"));
            foreach (var plus in otherDlls)
            {
                //如果网站bin目录不存在此dll，则将该dll复制到动态程序集目录
                if (binDir.GetFiles(plus.Name).Length == 0 && manager.DynamicDirectory.GetFiles(plus.Name).Length == 0)
                {
                    manager.CopyToDynamicDirectory(plus);
                    Assembly assembly = Assembly.LoadFrom(plus.FullName);
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    if (assemblies.All(p => p.FullName != assembly.FullName))
                        //将程序集添加到当前应用程序域
                        BuildManager.AddReferencedAssembly(assembly);
                }
            }
            var shoyDlls =
                plusDlls.Where(t => t.Name.StartsWith(PluginPrefix, StringComparison.InvariantCultureIgnoreCase));
            foreach (var plus in shoyDlls)
            {
                manager.Deploy(plus);
            }
        }

        private List<PluginAssembly> _pluginsList;
        /// <summary>
        /// 插件列表
        /// </summary>
        public override List<PluginAssembly> PluginsList
        {
            get
            {
                return _pluginsList;
            }
            set
            {
                _pluginsList = value;
            }
        }

        public override PluginAssembly GetPluginAssembly(FileInfo dllFile)
        {
            Assembly assembly = Assembly.LoadFrom(dllFile.FullName);
            return GetPluginAssembly(assembly, dllFile);
        }

        public override PluginAssembly GetPluginAssembly(Assembly assembly, FileInfo dllFile)
        {
            var plusInfo = GetPluginAssembly(assembly.FullName) ??
                           AssemblyManager.GetPlusAssemblysInfo(assembly, dllFile);
            if (PluginsList.All(p => p.FullName != assembly.FullName))
            {
                plusInfo.Installed = false;
                PluginsList.Add(plusInfo);
            }
            return plusInfo;
        }

        /// <summary> 部署程序集 </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public override Assembly Deploy(FileInfo dllFile)
        {
            var newDllFile = CopyToDynamicDirectory(dllFile);
            Assembly assembly = Assembly.LoadFrom(newDllFile.FullName);
            try
            {
                //将程序集添加到当前应用程序域
                BuildManager.AddReferencedAssembly(assembly);
                //执行插件初始化函数
                assembly.GetTypes().Where(p => p.IsClass && p.GetInterface(typeof(IPlugin).FullName) != null).Each(
                    t =>
                    {
                        try
                        {
                            var type = (IPlugin)Activator.CreateInstance(t);
                            type.Initialize();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(
                                string.Format("插件初始化失败！Assembly:{0}，Type:{1}{2}", assembly.FullName, t.FullName,
                                    Environment.NewLine), ex);
                        }
                    });
            }
            catch (FileLoadException ex)
            {
                throw new Exception(string.Format("加载此程序失败！Assembly：{0}，FileName：{1}", assembly.FullName, ex.FileName), ex);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    var exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                throw new Exception(errorMessage);
            }
            return assembly;
        }

        public override FileInfo CopyToDynamicDirectory(FileInfo dllFile)
        {
            var copyFolder = new DirectoryInfo(AppDomain.CurrentDomain.DynamicDirectory);
            var newDllFile = new FileInfo(Path.Combine(copyFolder.FullName, dllFile.Name));
            try
            {
                File.Copy(dllFile.FullName, newDllFile.FullName, true);
            }
            catch (Exception)//如果出现"正由另一进程使用，因此该进程无法访问该文件"错误，则先重命名再复制
            {
                try
                {
                    File.Move(newDllFile.FullName, newDllFile.FullName + Guid.NewGuid().ToString("N") + ".locked");
                }
                catch (Exception ioex)
                {
                    throw new Exception("部署插件程序集失败。PlusName：" + dllFile.Name, ioex);
                }
                File.Copy(dllFile.FullName, newDllFile.FullName, true);
            }
            return newDllFile;
        }

        public override void LoadPlusStrategys(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 安装或更新
        /// </summary>
        /// <param name="assembly"></param>
        public override void Install(Assembly assembly)
        {
            //在未安装的插件列表中获得对应插件
            var pluginInfo =
                PluginsList.FirstOrDefault(
                    p => p.FullName.Equals(assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
            //当插件为空时直接返回
            if (pluginInfo == null)
                throw new Exception(string.Format("插件[{0}]不存在", assembly.FullName));
            if (pluginInfo.Installed)
            {

            }
            else
            {
                pluginInfo.Installed = true;
            }
            XmlHelper.XmlSerializeToFile(PluginsList, PluginXmlPath, Encoding.UTF8);
        }

        public override void Uninstall(Assembly assembly)
        {
            //在未安装的插件列表中获得对应插件
            var pluginInfo = PluginsList.FirstOrDefault(p => p.FullName.Equals(assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
            //当插件为空时直接返回
            if (pluginInfo == null)
                throw new Exception(string.Format("插件[{0}]不存在", assembly.FullName));
            if (pluginInfo.Installed)
            {
                pluginInfo.Installed = false;
            }
            else
            {
                throw new Exception(string.Format("插件[{0}]尚未安装", assembly.FullName));
            }
            XmlHelper.XmlSerializeToFile(PluginsList, PluginXmlPath, Encoding.UTF8);
        }
    }
}
