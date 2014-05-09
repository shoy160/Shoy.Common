using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.IO;
using Shoy.Utility.Extend;

namespace Shoy.Utility
{
    public class EmailCls:IDisposable
    {
        private SmtpClient _smtpClient;
        private MailMessage _mailMsg;

        #region 构造函数

        public EmailCls(string senderEmail, string senderPwd,string senderName, string smtpHost,int smtpPort,bool useSsl)
        {
            CreateClient(senderEmail, senderPwd, senderName, smtpHost, smtpPort, useSsl);
        }

        public EmailCls(string senderEmail,string senderPwd,string senderName)
        {
            CreateClient(senderEmail, senderPwd, senderName, null, 0, false);
        }

        public EmailCls(string senderEmail, string senderPwd)
        {
            CreateClient(senderEmail, senderPwd, null, null, 0, false);
        }

        #endregion

        private void CreateClient(string senderEmail, string senderPwd, string senderName, string smtpHost, int smtpPort, bool useSsl)
        {
            if (string.IsNullOrEmpty(senderName))
                senderName = senderEmail;
            smtpPort = (smtpPort == 0 ? 25 : smtpPort);
            if(string.IsNullOrEmpty(smtpHost))
            {
                var host = senderEmail.Substring(senderEmail.IndexOf('@') + 1);
                smtpHost = "smtp." + host;
                switch (host)
                {
                    case "100hg.com":
                        smtpHost = "mail." + host;
                        break;
                    case "100hg.cn":
                        smtpHost = "smtp.exmail.qq.com";
                        break;
                    case "gmail.com":
                        useSsl = true;
                        smtpPort = 587;
                        break;
                    default:
                        break;
                }
            }
            _smtpClient = new SmtpClient
                              {
                                  Credentials = new NetworkCredential(senderEmail, senderPwd),
                                  EnableSsl = useSsl,
                                  Host = smtpHost,
                                  Port = smtpPort,
                                  Timeout = 1000*60*30
                              };
            _mailMsg = new MailMessage
                           {
                               From = new MailAddress(senderEmail, senderName, Encoding.UTF8),
                               SubjectEncoding = Encoding.UTF8,
                               BodyEncoding = Encoding.UTF8,
                               IsBodyHtml = true
                           };
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="receiver">收件人邮箱</param>
        /// <param name="title">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="files">邮件附件</param>
        /// <param name="async">是否异步发送</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool SendEmail(string receiver, string title, string body, List<string> files, bool  async, out string errMsg)
        {
            bool sendState;
            errMsg = "";
            if(receiver.IsNullOrEmpty())
            {
                errMsg = "没有收件人";
                return false;
            }
            try
            {
                _mailMsg.To.Clear();
                _mailMsg.To.Add(receiver);
                _mailMsg.Subject = title;
                _mailMsg.Body = body;
                _mailMsg.Attachments.Clear();
                if (files != null && files.Any())
                {
                    //添加附件
                    foreach (string attachmentFileName in files)
                    {
                        if (File.Exists(attachmentFileName))
                        {
                            var attachment = new Attachment(attachmentFileName);
                            _mailMsg.Attachments.Add(attachment);
                        }
                    }
                }

                //发邮件
                if (async)
                {
                    _smtpClient.SendCompleted += SmtpClientSendCompleted;
                    _smtpClient.SendAsync(_mailMsg, "");
                }
                else
                {
                    _smtpClient.Send(_mailMsg);
                }
                sendState = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                sendState = false;
            }
            return sendState;
        }

        void SmtpClientSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var path = Utils.GetMapPath("/email.log");
            if (e.Error != null)
            {
                Utils.WriteException(e.Error);
                Utils.WriteFile(path, "发送失败：" + e.Error.Message + "-->" + Utils.GetTimeNow());
            }
            else
            {
                Utils.WriteFile(path, "发送成功！-->" + Utils.GetTimeNow());
            }
            _smtpClient.SendCompleted -= SmtpClientSendCompleted;
        }

        #region 简化

        /// <summary>
        /// (简化)发送邮件
        /// </summary>
        /// <param name="receiver">收件人邮箱</param>
        /// <param name="title">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool SendEmail(string receiver, string title, string body, out string errMsg)
        {
            return SendEmail(receiver, title, body, null,false, out errMsg);
        }

        /// <summary>
        /// (简化)发送邮件
        /// </summary>
        /// <param name="receiver">收件人邮箱</param>
        /// <param name="title">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <returns></returns>
        public bool SendEmail(string receiver, string title, string body)
        {
            string msg;
            return SendEmail(receiver, title, body, null, false, out msg);
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (_mailMsg != null)
                _mailMsg.Dispose();
        }

        #endregion
    }
}
