
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Shoy.Core.Config;
using Shoy.Utility.Config;

namespace Shoy.Core
{
    public static class Consts
    {
        public const string Version = "3.0.0.0";
        /// <summary> 用户登录Cookie </summary>
        public const string UserCookieName = "__dayeasy_u";

        /// <summary> 登录错误次数 </summary>
        public const string LoginCountCookieName = "__dayeasy_err_c";

        public static readonly Func<Assembly, bool> AssemblyFinder =
            t => t.FullName.StartsWith("shoy.", StringComparison.CurrentCultureIgnoreCase);

        public static SiteConfig Config
        {
            get { return ConfigUtils<SiteConfig>.Instance.Get(); }
        }

        /// <summary> 选项字母集 </summary>
        public static string[] OptionWords
        {
            get
            {
                var list = new List<string>();
                for (var i = 65; i < 91; i++)
                    list.Add(Convert.ToChar(i).ToString(CultureInfo.InvariantCulture));
                return list.ToArray();
            }
        }

        public static DateTime DefaultTime = new DateTime(1970, 1, 1);

        public static DateTime ToDateTime(this long time)
        {
            if (time <= 0) return DefaultTime;
            return new DateTime(DefaultTime.Ticks + time * 10000);
        }

        public static long ToLong(this DateTime time)
        {
            if (time <= DefaultTime) return 0;

            return (time.Ticks - DefaultTime.Ticks) / 10000;
        }
    }
}
