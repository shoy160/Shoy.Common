using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.SessionState;
using Shoy.AjaxHelper.Model;
using System.Text;
using Shoy.Utility.Extend;

namespace Shoy.AjaxHelper.Core
{
    /// <summary>
    /// 反射方法 辅助类
    /// </summary>
    internal class MethodHelper:IRequiresSessionState,IReadOnlySessionState
    {
        private static readonly object LockObj = new object();

        private static readonly IDictionary<string, Assembly> AssemblyCache =
            new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

        private static readonly IDictionary<string, Type> ClassCaceh =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private static readonly IDictionary<string, CustomMethodInfo> MethodCache =
            new Dictionary<string, CustomMethodInfo>(StringComparer.OrdinalIgnoreCase);

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        private readonly HttpContext _context;

        private readonly MethodPathInfo _pathInfo;

        private HttpRequestInfo _request;

        static MethodHelper()
        {
            InitCache();
        }

        public MethodHelper(HttpContext context, MethodPathInfo info)
        {
            if (context == null)
                throw new AjaxException("没有请求上下文！");
            if (!info.IsValidate)
                throw new AjaxException("请求路径错误！");
            _context = context;
            _pathInfo = info;
        }

        public CustomMethodInfo GetMethod()
        {
            var key = _pathInfo.MethodKey;
            CustomMethodInfo info = null;
            //从缓存中取
            if (MethodCache.Keys.Contains(key))
            {
                info = MethodCache[key];
                info.LastUpdateTime = DateTime.Now;
                info.Count++;
                MethodCache[key] = info;
            }
            else
            {
                //无缓存
                info = GetMethodFromPath();
                if (info == null)
                    throw new AjaxException("没有找到方法:" + _pathInfo.MethodName);
                if (!MethodCache.Keys.Contains(key))
                {
                    lock (LockObj)
                    {
                        if (!MethodCache.Keys.Contains(key))
                            MethodCache.Add(key, info);
                    }
                }
            }
            var action = info.AttrList.Find(t => t is AjaxAction) as AjaxAction;
            if (action == null)
                throw new AjaxException("不能通过网络访问该方法");
            _request = new HttpRequestInfo
                           {
                               Context = _context,
                               CurrentMethodInfo = info,
                               WebParameters = action.GetWebParameters(_context)
                           };

            if (!CheckAttributs(info.AttrList))
                info = null;

            return info;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public object ExecutinonMethod(CustomMethodInfo info)
        {
            try
            {
                object obj = null;
                if (info.ParamterInfos.Length == 0)
                {
                    obj = info.Method.Invoke(info.Instance, null);
                }
                else
                {
                    var args = new object[info.ParamterInfos.Length];
                    for (int i = 0; i < info.ParamterInfos.Length; i++)
                    {
                        var para = info.ParamterInfos[i];
                        var val = _request.WebParameters[para.Name];
                        args[i] = ReflectionHelper.ChangeType(val, para.ParameterType);
                    }
                    obj = info.Method.Invoke(info.Instance, args);
                }
                return obj;
            }
            catch(Exception ex)
            {
                return new AjaxResult {state = 0, msg = ex.Message};
            }
        }

        /// <summary>
        /// 验证自定义属性
        /// </summary>
        /// <returns></returns>
        private bool CheckAttributs(IEnumerable<AttrBase> attrs)
        {
            var result = true;
            foreach (AttrBase attr in attrs)
            {
                attr.CurrentHttpRequest = _request;
                attr.MethodPath = _pathInfo;
                if (!attr.IsValidate())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private CustomMethodInfo GetMethodFromPath()
        {
            var info = new CustomMethodInfo();
            try
            {
                info.Assembly = Assembly.Load(_pathInfo.Assembly);
                var type = info.Assembly.GetType(_pathInfo.TypeName, true, true);
                info.Instance = Activator.CreateInstance(type);
                info.Method = type.GetMethod(_pathInfo.MethodName, Flags);
                info.Count = 1;
                info.AttrList = ReflectionHelper.GetAttributes<AttrBase>(info.Method);
                info.ReturnType = info.Method.ReturnType;
                info.ParamterInfos = info.Method.GetParameters();
                info.LastUpdateTime = DateTime.Now;
            }
            catch
            {
                info = null;
            }
            return info;
        }

        /// <summary>
        /// 初始化缓存
        /// </summary>
        private static void InitCache()
        {
            var asses = BuildManager.GetReferencedAssemblies();
            //添加程序集 和 类的缓存
            foreach (Assembly ass in asses)
            {
                var name = GetAssemblyName(ass);
                if (UrlHelper.Assembly.Equals(name))
                {
                    if (!AssemblyCache.Keys.Contains(name))
                        AssemblyCache.Add(name, ass);
                    foreach (Type type in ass.GetExportedTypes())
                    {
                        var key = type.FullName;
                        if (string.IsNullOrEmpty(key))
                            continue;
                        var faces = type.GetInterfaces();
                        foreach (Type t in faces)
                        {
                            if (t.Name == "IAjax" && !ClassCaceh.Keys.Contains(key))
                            {
                                ClassCaceh.Add(key, type);
                            }
                        }
                    }
                }
            }
            //添加方法的缓存
            foreach (var cls in ClassCaceh.Keys)
            {
                var type = ClassCaceh[cls];
                foreach (MethodInfo info in type.GetMethods(Flags))
                {
                    var key = cls + "." + info.Name;
                    if (!MethodCache.Keys.Contains(key))
                    {
                        var list = ReflectionHelper.GetAttributes<AttrBase>(info);
                        var action = list.Find(t => t is AjaxAction) as AjaxAction;
                        if (action == null) continue;
                        MethodCache.Add(key, new CustomMethodInfo
                                                 {
                                                     AttrList = list,
                                                     Count = 0,
                                                     LastUpdateTime = DateTime.Now,
                                                     Method = info,
                                                     ReturnType = info.ReturnType,
                                                     ParamterInfos = info.GetParameters(),
                                                     Instance = Activator.CreateInstance(type),
                                                     Assembly = type.Assembly
                                                 });
                    }
                }
            }

        }

        private static string GetAssemblyName(Assembly ass)
        {
            return ass.FullName.Split(',')[0];
        }

        public static string ShowMethods()
        {
            var sb = new StringBuilder();
            foreach (var info in MethodCache.Keys)
            {
                sb.AppendLine(info);
                var item = MethodCache[info];
                sb.AppendLine("count:{0},date:{1}".FormatWith(item.Count,
                                                              item.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            return sb.ToString();
        }
    }
}
