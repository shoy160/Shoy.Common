using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Shoy.FileCompare
{
    public partial class PublisherHelper : Form
    {
        private string _lastTimePath;
        private Stopwatch _watch;
        private Dictionary<Regex, string> _regexDict;
        private string _sourcePath;
        private string _targetPath;
        private string[] _fileExts;
        private DateTime _lastTime;

        public PublisherHelper()
        {
            InitializeComponent();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker.DoWork += bgWorker_DoWork;
            LastDate.Format = DateTimePickerFormat.Custom;
            LastDate.CustomFormat = @"yyyy-MM-dd HH:mm:ss";
            SourceDir.Text = "sourceDirectory".Config();
            TargetDir.Text = "baseTargetDirectory".Config();
            FileExts.Text = "fileExts".Config();
            var rules = "replaceRules".Config();
            ReplaceRules.Text = string.Join("\n",
                new Regex("[\r\n]").Split(rules).Where(t => !string.IsNullOrWhiteSpace(t.Trim())).Select(t => t.Trim()));

            var forder = new FolderBrowserDialog();
            SourceDir.DoubleClick += (e, arg) =>
            {
                forder.SelectedPath = SourceDir.Text;
                if (forder.ShowDialog() == DialogResult.OK)
                    SourceDir.Text = forder.SelectedPath;
            };
            TargetDir.DoubleClick += (e, arg) =>
            {
                forder.SelectedPath = TargetDir.Text;
                if (forder.ShowDialog() == DialogResult.OK)
                    TargetDir.Text = forder.SelectedPath;
            };
            InitConfig();
        }

        private void InitConfig()
        {
            _watch = new Stopwatch();
            _lastTimePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Const.LastTimeFile);
            if (!File.Exists(_lastTimePath)) return;
            var date = File.ReadAllText(_lastTimePath);
            if (!string.IsNullOrWhiteSpace(date))
            {
                LastDate.Value = DateTime.Parse(date);
            }
        }

        private void SetLastDate(DateTime date)
        {
            File.WriteAllText(_lastTimePath, date.ToString(CultureInfo.InvariantCulture));
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(_sourcePath) || !Directory.Exists(_targetPath))
            {
                bgWorker.ReportProgress(0, "文件夹不存在");
                return;
            }
            _targetPath = Path.Combine(_targetPath, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            CheckFile(_sourcePath, _targetPath);
        }

        private void CheckFile(string path, string savePath)
        {
            var flist = new DirectoryInfo(path).GetFiles();
            flist = flist.Where(t => t.LastWriteTime >= _lastTime).ToArray();
            if (flist.Length > 0)
            {
                foreach (var info in flist)
                {
                    if (!info.Exists)
                        continue;
                    var progress = $"{info.Name}\t\t->最后修改时间：{info.LastWriteTime:yyy-MM-dd HH:mm:ss}";
                    bgWorker.ReportProgress(0, progress);
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    var name = info.Name.DomainReplace(_regexDict).Item2;
                    var itemPath = Path.Combine(savePath, name);
                    info.CopyTo(itemPath);
                    ReplaceMission(itemPath);
                }
            }
            var ds = Directory.GetDirectories(path);
            if (ds.Length <= 0) return;
            foreach (var d in ds)
            {
                var dInfo = new DirectoryInfo(d);
                if (dInfo.Exists)
                {
                    CheckFile(d, Path.Combine(savePath, dInfo.Name.DomainReplace(_regexDict).Item2));
                }
            }
        }

        /// <summary> 文本替换 </summary>
        /// <param name="path"></param>
        private void ReplaceMission(string path)
        {
            if (!File.Exists(path) || _regexDict == null || !_regexDict.Any())
                return;
            var ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext) || !_fileExts.Contains(ext))
                return;
            var txt = File.ReadAllText(path, Encoding.UTF8);
            var tuple = txt.DomainReplace(_regexDict);
            if (!tuple.Item1) return;
            File.WriteAllText(path, tuple.Item2, Encoding.UTF8);
            var progress = $"文件：{path}，被替换";
            bgWorker.ReportProgress(0, progress);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (rtbLog.Lines.Length > 2000)
                rtbLog.Clear();
            rtbLog.AppendText($"{e.UserState}\r\n");
            rtbLog.Select(rtbLog.TextLength - 1, 1);
            rtbLog.ScrollToCaret();
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _watch.Stop();
            rtbLog.AppendText($"共耗时：{_watch.ElapsedMilliseconds}ms\r\n");
            _watch.Reset();
            rtbLog.AppendText("Mission Complete!\r\n");
            SetLastDate(DateTime.Now);
            SetStatus(true);
        }

        private void SetStatus(bool status)
        {
            SourceDir.Enabled = status;
            TargetDir.Enabled = status;
            FileExts.Enabled = status;
            ReplaceRules.Enabled = status;
            btnAction.Enabled = status;
            LastDate.Enabled = status;
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            _watch.Start();
            _sourcePath = SourceDir.Text;
            _targetPath = TargetDir.Text;
            _fileExts = FileExts.Text.FileExts();
            _lastTime = LastDate.Value;
            _regexDict = ReplaceRules.Text.ReplaceRegex();
            SetStatus(false);
            bgWorker.RunWorkerAsync();
        }
    }
}
