using System.Configuration;
using System.Text.RegularExpressions;
using Shoy.AjaxHelper.Model;

namespace Shoy.AjaxHelper.Core
{
    internal class UrlHelper
    {
        private const string RegexText = @"^[/](?<class>[a-z0-9-_]+([/][a-z0-9-_]+)*)[/\.](?<method>[a-z0-9-_]+)(\.[a-z0-9]{1,6})?$";

        public static readonly string Assembly = "Shoy.AjaxHelper";

        static UrlHelper()
        {
            string assembly = ConfigurationManager.AppSettings["AjaxNamespace"];
            if (string.IsNullOrEmpty(assembly))
            {
                throw new AjaxException("assembly is null");
            }
            Assembly = assembly;
        }

        private static bool HasAssembly(string assmebly)
        {
            try
            {
                var ass = System.Reflection.Assembly.Load(assmebly);
                return ass != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 得到方法的一些基本的路径信息
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static MethodPathInfo GetMethodPathInfo(string virtualPath)
        {
            MethodPathInfo methodPathInfo = null;

            Match match = Regex.Match(virtualPath, RegexText, RegexOptions.IgnoreCase);
            //如果匹配到了
            if (match.Success)
            {
                //取出class和method
                methodPathInfo = new MethodPathInfo
                                     {
                                         ClassName = match.Groups["class"].Value.Replace("/", "."),
                                         MethodName = match.Groups["method"].Value,
                                         Assembly = Assembly
                                     };
            }
            return methodPathInfo;
        }
    }
}
