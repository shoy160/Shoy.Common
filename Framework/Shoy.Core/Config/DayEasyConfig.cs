using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.Core.Config
{
    /// <summary> 得一平台全站基础配置 </summary>
    [Serializable]
    [XmlRoot("dayeasy")]
    [FileName("dayeasy.config")]
    public class DayEasyConfig : ConfigBase
    {
        [XmlAttribute("isOnline")]
        public bool IsOnline { get; set; }

        /// <summary>
        /// CookieDomain配置
        /// </summary>
        [XmlElement("cookieDomain")]
        public string CookieDomain { get; set; }

        /// <summary>
        /// 静态资源站点
        /// </summary>
        [XmlElement("staticUrl")]
        public string StaticUrl { get; set; }

        /// <summary>
        /// 主站站点
        /// </summary>
        [XmlElement("mainUrl")]
        public string MainUrl { get; set; }

        /// <summary>
        /// 登录站点
        /// </summary>
        [XmlElement("loginUrl")]
        public string LoginUrl { get; set; }

        /// <summary>
        /// 注册站点
        /// </summary>
        [XmlElement("registerUrl")]
        public string RegisterUrl { get; set; }

        /// <summary>
        /// 后台管理站点
        /// </summary>
        [XmlElement("adminUrl")]
        public string AdminUrl { get; set; }

        /// <summary>
        /// 接口站点
        /// </summary>
        [XmlElement("openUrl")]
        public string OpenUrl { get; set; }

        /// <summary>
        /// 课堂站点
        /// </summary>
        [XmlElement("classroom")]
        public string Classroom { get; set; }

        [XmlElement("org")]
        public string OrganizationSite { get; set; }

        /// <summary>
        /// 文件站点
        /// </summary>
        [XmlElement("fileSite")]
        public string FileSite { get; set; }

        /// <summary>
        /// 邮件配置
        /// </summary>
        [XmlElement("email")]
        public EmailConfig Email { get; set; }

        ///// <summary>
        ///// 注册邮件模板
        ///// </summary>
        //[XmlElement("regEmailTep")]
        //public string RegEmailTep { get; set; }

        ///// <summary>
        ///// 找回密码邮件模板
        ///// </summary>
        //[XmlElement("findPwdEmailTep")]
        //public string FindPwdEmailTep { get; set; }

        [XmlArray("templates")]
        [XmlArrayItem("item")]
        public List<MessageTemplate> MessageTemplates { get; set; }

        /// <summary>
        /// 静态资源时间戳
        /// </summary>
        [XmlElement("staticTick")]
        public string StaticTick { get; set; }

        /// <summary>
        /// 得一帐号 - 分派试卷使用
        /// </summary>
        [XmlElement("deyiAccount")]
        public string DeyiAccount { get; set; }

        /// <summary>
        /// 公式图片存储路径
        /// </summary>
        [XmlElement("latexPath")]
        public string LatexPath { get; set; }

        /// <summary> 日志记录级别 </summary>
        [XmlElement("logLevel")]
        public string LogLevel { get; set; }
    }

    /// <summary>
    /// 邮件配置
    /// </summary>
    [Serializable]
    public class EmailConfig
    {
        /// <summary>
        /// 发件邮箱
        /// </summary>
        [XmlAttribute("senderEmail")]
        public string SenderEmail { get; set; }

        /// <summary>
        /// 发件邮箱密码
        /// </summary>
        [XmlAttribute("senderPwd")]
        public string SenderPwd { get; set; }

        /// <summary>
        /// 发件人
        /// </summary>
        [XmlAttribute("senderName")]
        public string SenderName { get; set; }

        /// <summary>
        /// smtpHost
        /// </summary>
        [XmlAttribute("smtpHost")]
        public string SmtpHost { get; set; }

        /// <summary>
        /// smtpPort
        /// </summary>
        [XmlAttribute("smtpPort")]
        public int SmtpPort { get; set; }

        /// <summary>
        /// 是否使用加密连接
        /// </summary>
        [XmlAttribute("useSsl")]
        public bool UseSsl { get; set; }
    }

    /// <summary>
    /// 消息模版配置
    /// </summary>
    [Serializable]
    public class MessageTemplate
    {
        /// <summary>
        /// 模版类型
        /// </summary>
        [XmlAttribute("type")]
        public int MessageType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [XmlAttribute("title")]
        public string Title { get; set; }

        /// <summary>
        /// 模版
        /// </summary>
        [XmlText]
        public string Template { get; set; }
    }
}
