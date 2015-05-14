using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Shoy.Utility.Helper;

namespace Shoy.Utility.Extend
{
    public interface IRegex : IExtension<string> { }

    public static class RegexExtension
    {
        public static Regex ToRegex(this IRegex c, RegexOptions opts)
        {
            return new Regex(c.GetValue(), opts);
        }

        public static Regex ToRegex(this IRegex c)
        {
            return new Regex(c.GetValue(), RegexOptions.Compiled);
        }

        public static bool IsMatch(this IRegex c, string pattern, RegexOptions opts)
        {
            if (c.GetValue().IsNullOrEmpty()) return false;
            return Regex.IsMatch(c.GetValue(), pattern, opts);
        }

        public static bool IsMatch(this IRegex c, string pattern)
        {
            return c.IsMatch(pattern, RegexOptions.None);
        }

        public static string Match(this IRegex c, string pattern)
        {
            return c.Match(pattern, 0);
        }

        public static string Match(this IRegex c, string pattern, int index)
        {
            return c.Match(pattern, index, RegexOptions.None);
        }

        public static string Match(this IRegex c, string pattern, int index, RegexOptions opts)
        {
            return c.GetValue().IsNullOrEmpty() ? string.Empty : Regex.Match(c.GetValue(), pattern, opts).Groups[index].Value;
        }

        public static string Match(this IRegex c, string pattern, string groupName, RegexOptions opts)
        {
            return c.GetValue().IsNullOrEmpty() ? string.Empty : Regex.Match(c.GetValue(), pattern, opts).Groups[groupName].Value;
        }

        public static string Match(this IRegex c, string pattern, string groupName)
        {
            return c.Match(pattern, groupName, RegexOptions.None);
        }

        public static IEnumerable<string> Matches(this IRegex c, string parent)
        {
            return c.Matches(parent, 1, RegexOptions.None);
        }

        public static IEnumerable<string> Matches(this IRegex c, string parent, int index, RegexOptions opts)
        {
            var list = new List<string>();
            if (c.GetValue().IsNullOrEmpty())
                return list;
            var ms = Regex.Matches(c.GetValue(), parent, opts);
            list.AddRange(from Match m in ms select m.Groups[index].Value);
            return list;
        }

        public static IEnumerable<string> Matches(this IRegex c, string parent, string groupName, RegexOptions opts)
        {
            var list = new List<string>();
            if (c.GetValue().IsNullOrEmpty())
                return list;
            var ms = Regex.Matches(c.GetValue(), parent, opts);
            list.AddRange(from Match m in ms select m.Groups[groupName].Value);
            return list;
        }

        public static string Replace(this IRegex c, string parent, string replaceMent, int count, int startAt, RegexOptions opts)
        {
            var reg = parent.As<IRegex>().ToRegex(opts);
            if (count <= 0)
                return reg.Replace(c.GetValue(), replaceMent);
            return startAt >= 0
                ? reg.Replace(c.GetValue(), replaceMent, count, startAt)
                : reg.Replace(c.GetValue(), replaceMent, count);
        }

        public static string Replace(this IRegex c, string parent, string replaceMent, RegexOptions opts)
        {
            return c.Replace(parent, replaceMent, 0, -1, opts);
        }

        public static string Replace(this IRegex c, string parent, string replaceMent)
        {
            return c.Replace(parent, replaceMent, RegexOptions.Compiled);
        }

        //常用正则
        public static bool IsEmail(this IRegex c)
        {
            return RegexHelper.IsEmail(c.GetValue());
        }

        public static bool IsIp(this IRegex c)
        {
            return RegexHelper.IsIp(c.GetValue());
        }

        public static bool IsUrl(this IRegex c)
        {
            return RegexHelper.IsUrl(c.GetValue());
        }

        public static bool IsMobile(this IRegex c)
        {
            return RegexHelper.IsMobile(c.GetValue());
        }
    }
}
