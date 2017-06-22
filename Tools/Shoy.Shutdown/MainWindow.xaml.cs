using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Deyi.Shutdown.Core;

namespace Deyi.Shutdown
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OperateLog.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            OperateLog.Document.Blocks.Clear();
            InitServers();
        }

        private void InitServers()
        {
            var list = new List<string>();
            var config = ConfigurationManager.AppSettings.Get("servers");
            if (!string.IsNullOrWhiteSpace(config))
                list = config.Split(',').ToList();
            ServerList.ItemsSource = list.Select(t => new CheckItemData
            {
                Name = t,
                IsEnabled = true
            }).ToList();
        }

        private void AppendLog(string msg, bool success = true)
        {
            var paragraph = new Paragraph
            {
                Foreground = new SolidColorBrush(success ? Colors.Green : Colors.Red),
                Margin = new Thickness(5)
            };
            paragraph.Inlines.Add(string.Format("{0}:\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));

            Dispatcher.Invoke(() =>
            {
                OperateLog.Document.Blocks.Add(paragraph);
                OperateLog.ScrollToEnd();
            });
        }

        private void MissionAction(IEnumerable<CheckItemData> servers, bool shutdown = true)
        {
            string userName = ConfigurationManager.AppSettings.Get("userName"),
                userPwd = ConfigurationManager.AppSettings.Get("userPwd");
            if (!string.IsNullOrWhiteSpace(userPwd))
            {
                userPwd = SecurityCls.Decode(userPwd);
            }
            foreach (var server in servers)
            {
                var result = PreAction(userName, userPwd, server.Name, shutdown);
                if (result.Status)
                    AppendLog(string.Format("{0}{1}成功！", server.Name, shutdown ? "关机" : "重启"));
                else
                {
                    AppendLog(string.Format("{0}:{1}！", server.Name, result.Message), false);
                }
            }
        }

        private DResult PreAction(string userName, string userPwd, string ip, bool shutdown = true)
        {
            //return new Random().Next(0, 10) % 3 == 0 ? DResult.Success : DResult.Error("链接失败！");
            var options = new ConnectionOptions { Username = userName, Password = userPwd };
            var scope = new ManagementScope("\\\\" + ip + "\\root\\cimv2", options);
            try
            {
                //用给定管理者用户名和口令连接远程的计算机
                scope.Connect();
                var oq = new ObjectQuery("select * from win32_OperatingSystem");
                var query = new ManagementObjectSearcher(scope, oq);
                var queryCollection = query.Get();
                foreach (var o in queryCollection)
                {
                    var mo = (ManagementObject)o;
                    mo.InvokeMethod(shutdown ? "Shutdown" : "Reboot", null);
                    //注销
                    //mo.InvokeMethod("Logoff", null);
                }
                return DResult.Success;
            }
            catch (Exception er)
            {
                return DResult.Error("连接" + ip + "出错，出错信息为：" + er.Message);
            }
        }

        private void EnabledBtns(bool enabled = true)
        {
            Dispatcher.Invoke(() =>
            {
                BtnRestart.IsEnabled = enabled;
                BtnShutdown.IsEnabled = enabled;
                ServerList.IsEnabled = enabled;
                CheckAll.IsEnabled = enabled;
            });
        }

        private void BtnAction(bool shutdown = true)
        {
            var servers = ServerList.SelectedItems;
            if (servers == null || servers.Count == 0)
            {
                MessageBox.Show("没有选择服务器", "消息提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EnabledBtns(false);
            var list = servers.Cast<CheckItemData>().ToList();
            //            Task.Factory.StartNew(() =>
            {
                MissionAction(list, shutdown);
                EnabledBtns();
            }//);
        }

        private void BtnRestartClick(object sender, RoutedEventArgs e)
        {
            BtnAction(false);
        }

        private void BtnShutdownClick(object sender, RoutedEventArgs e)
        {
            BtnAction();
        }
    }
}
