using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shoy.FileCompare
{
    public static class Const
    {
        internal const string LastTimeFile = "last_time.db";

        internal static string Config(this string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        internal static string[] FileExts(this string fileExts)
        {
            return fileExts.Split(new[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static Dictionary<Regex, string> ReplaceRegex(this string replaceRules)
        {
            var rules =
                new Regex("[\r\n]").Split(replaceRules).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim());
            var regDict = new Dictionary<Regex, string>();
            foreach (var rule in rules)
            {
                var item = rule.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                if (item.Length != 2)
                    continue;
                regDict.Add(new Regex(item[0], RegexOptions.IgnoreCase | RegexOptions.Multiline), item[1]);
            }
            return regDict;
        }

        internal static Tuple<bool, string> DomainReplace(this string str, Dictionary<Regex, string> replaceList)
        {
            if (string.IsNullOrWhiteSpace(str) || replaceList == null || !replaceList.Any())
                return new Tuple<bool, string>(false, str);
            if (!replaceList.Keys.Any(t => t.IsMatch(str)))
                return new Tuple<bool, string>(false, str);
            foreach (var rep in replaceList)
            {
                str = rep.Key.Replace(str, rep.Value);
            }
            return new Tuple<bool, string>(true, str);
        }
    }
}
