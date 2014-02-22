using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Shoy.Utility.Extend;
using System.Linq;

namespace Shoy.Utility
{
    /// <summary>
    /// html模板类
    /// version 1.0.1
    /// create at 2013-06-01 author shy
    /// 标签:
    /// <h:for data="" item="">@item,@data</h:for>
    /// <h:if cond=""></h:if><h:else cond=""></h:else>
    /// </summary>
    public class HTemplate
    {
        /// <summary>
        /// 模板标签，形如 &lt;ht:template header&gt;
        /// </summary>
        private const string TemplateTag = "<ht:template\\s+([0-9a-z_-]+)>";

        /// <summary>
        /// 引用标签，形如&lt;ht:include header&gt;
        /// </summary>
        private const string IncludeTag = "<ht:include\\s+([0-9a-z_-]+)>";

        private const string Tag = "<ht:{0}>([\\w\\W]+?)</ht:{0}>";

        /// <summary>
        /// 数据源标签
        /// </summary>
        private const string DataTag = "<ht:#{0}>([\\w\\W]+?)</ht:#{0}>";

        private const string CDataTag = "<ht:#([0-9a-z_-]+)>([\\w\\W]+?)</ht:#\\1>";

        /// <summary>
        /// 属性值标签
        /// </summary>
        private const string AttrTag = "@(?:\\{?)([0-9a-z_-]+)(\\}?)";

        private readonly string _basePath;
        private readonly string _tmpDirectory;
        private readonly string _pageName;
        private readonly string _outPath;
        private string _html;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="basePath">模板根目录</param>
        /// <param name="tmpDirectory">模板目录，为空，表示同根目录</param>
        /// <param name="pageName">页面名称，含扩展名</param>
        /// <param name="outPath">输入路径</param>
        public HTemplate(string basePath, string tmpDirectory, string pageName, string outPath)
        {
            _basePath = basePath;
            if (tmpDirectory.IsNullOrEmpty())
                _tmpDirectory = basePath;
            else
                _tmpDirectory = basePath + "\\" + tmpDirectory;
            _pageName = pageName;
            _outPath = outPath;
        }

        /// <summary>
        /// 执行基础任务
        /// </summary>
        public void MissionStart()
        {
            //加载模板页
            var page = _basePath + "\\" + _pageName;
            if (!File.Exists(page))
                return;
            var version = "<!-- 本页面由HTemplate生成于{0} -->\r\n".FormatWith(Utils.GetTimeNow());
            _html = LoadHtml(page) + version;
            if(_html.IsNullOrEmpty())
                return;
            //加载模板
            var tmps = Matches(_html, TemplateTag);
            foreach (Match tmp in tmps)
            {
                var tmpName = tmp.Groups[1].Value;
                var tmpHtml = LoadHtml(_tmpDirectory + "\\" + tmpName + ".htm");
                _html = _html.Replace(tmp.Value, tmpHtml);
            }

            //加载Include标签
            var includes = Matches(_html, IncludeTag);
            foreach (Match include in includes)
            {
                var reg = Tag.FormatWith(include.Groups[1].Value);
                var tags = Matches(_html, reg);
                var html = "";
                foreach (Match tag in tags)
                {
                    html += Utils.ClearTrn(tag.Groups[1].Value);
                    _html = _html.Replace(tag.Value, "");
                }
                _html = _html.Replace(include.Value, html);
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="t"></param>
        public void BindData<T>(string name,T t)
        {
            var reg = DataTag.FormatWith(name);
            var datas = Matches(_html, reg);
            var type = t.GetType();
            foreach (Match data in datas)
            {
                var html = "";
                var tmpHtml = data.Groups[1].Value;//数据源模板
                //绑定属性
                //集合
                var list = t as ICollection;
                if (type.IsGenericType && list != null)
                {
                    foreach (var item in list)
                    {
                        html += BindAttr(tmpHtml, item, true);
                    }
                }
                else
                {
                    html += BindAttr(tmpHtml, t, true);
                }

                _html = _html.Replace(data.Value, html);
            }
        }

        /// <summary>
        /// 绑定子数据源
        /// </summary>
        /// <param name="tmpHtml">模板html</param>
        /// <param name="obj">子数据源</param>
        /// <returns></returns>
        private static string BindCData(string tmpHtml,object obj)
        {
            var html = tmpHtml;
            return html;
        }

        /// <summary>
        /// 绑定属性值
        /// </summary>
        /// <param name="html">模板html</param>
        /// <param name="obj">数据源</param>
        /// <param name="isTop">是否是最外层数据,是的话会去掉数据源中的无效数据</param>
        /// <returns></returns>
        private static string BindAttr(string html, object obj,bool isTop)
        {
            if (obj == null || obj.Equals(""))
                return "";
            var type = obj.GetType();
            var cDatas = Matches(html, CDataTag);
            //绑定子数据
            if (cDatas.Count > 0)
            {
                foreach (Match cData in cDatas)
                {
                    var name = cData.Groups[1].Value;
                    var ps = type.GetProperty(name);
                    if (ps != null)
                    {
                        var val = ps.GetValue(obj, null);
                        if (val == null)
                        {
                            html = html.Replace(cData.Value, "");
                        }
                        else
                        {
                            var tmp = cData.Groups[2].Value;
                            var cHtml = "";
                            var cType = val.GetType();
                            var list = val as ICollection;
                            if (cType.IsGenericType && list != null)
                            {
                                foreach (var item in list)
                                {
                                    cHtml += BindAttr(tmp, item,false);
                                }
                            }
                            else
                            {
                                cHtml += BindAttr(tmp, val, false);
                            }
                            html = html.Replace(cData.Value, cHtml);
                        }
                    }
                }
            }
            var attrs = Matches(html, AttrTag);
            foreach (Match attr in attrs)
            {
                var ps = type.GetProperty(attr.Groups[1].Value);
                if (ps != null)
                {
                    var val = ps.GetValue(obj, null) + "";
                    html = html.Replace(attr.Value, val);
                }
                else if (isTop)
                {
                    html = html.Replace(attr.Value, "");
                }
            }
            return html;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public void WriteToFile()
        {
            Utils.WriteFile(_outPath, _html, false, Encoding.UTF8);
        }

        /// <summary>
        /// 获取html代码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            return _html;
        }

        private static MatchCollection Matches(string str,string parent)
        {
            return Regex.Matches(str, parent, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        /// <summary>
        /// 读取html
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string LoadHtml(string path, Encoding encoding)
        {
            var html = "";
            if (File.Exists(path))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(path, encoding);
                    html = sr.ReadToEnd();
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                }
            }
            return html;
        }

        /// <summary>
        /// 读取html
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string LoadHtml(string path)
        {
            return LoadHtml(path, Encoding.Default);
        }
    }
}
