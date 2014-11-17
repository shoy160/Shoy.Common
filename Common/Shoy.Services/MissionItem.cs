using System;
using System.Diagnostics;
using System.Threading;

namespace Shoy.Services
{
    public class MissionItem
    {
        private readonly IMission _mission;
        private readonly Mission _mc;
        private AutoResetEvent _autoEvent;
        private readonly Thread _th;
        //private readonly Logger _logger = Logger.L<MissionItem>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mc"></param>
        public MissionItem(Mission mc)
        {
            _mc = mc;
            _autoEvent = new AutoResetEvent(false);
            var tp = Type.GetType(mc.Type);
            if (tp == null)
            {
                //_logger.W(string.Format("没有找到类型 {0} ", mc.Type));
                return;
            }
            var obj = Activator.CreateInstance(tp);
            if (!(obj is IMission))
            {
                //_logger.W(string.Format("类型 {0} 创建实例失败或没有实现 IMission 接口", mc.Type));
                return;
            }
            _mission = (IMission) obj;
            _mission.Error += mission_Error;
            var ths = new ThreadStart(Run);
            _th = new Thread(ths);
        }

        private void mission_Error(object sender, ErrorEventArg arg)
        {
            //_logger.E(arg.Message);
            arg.Cancel = false;
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public void Start()
        {
            if (_th != null && !_th.IsAlive)
                _th.Start();
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        public void Stop()
        {
            if (_th != null && _th.IsAlive)
            {
                _mission.Abort();
                if (_autoEvent != null)
                    _autoEvent.Set();
                _th.Join(5000);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void Run()
        {
            int i = 0;
            int interval;
            while (true)
            {
                i++;
                //_logger.I(string.Format("{0} 开始第 {1} 次执行任务。", _mc.Name, i));
                try
                {
                    var watcher = new Stopwatch();
                    watcher.Start();
                    _mission.Action();
                    watcher.Stop();
                    //_logger.I(string.Format("{0} 第 {1} 次执行任务完毕，耗时 {2} 毫秒。", _mc.Name, i,
                    //    watcher.ElapsedMilliseconds));
                }
                catch (Exception e)
                {
                    //_logger.E(string.Format("{0} 任务执行出错 {1}。{2}", _mc.Name, this, e.Message));
                }
                interval = _mc.Interval*60*1000;
                //每日 18点到次日6点之间等待时间延长至5倍
                if ((DateTime.Now.Hour < 6) || (DateTime.Now.Hour > 18))
                {
                    if (_mc.Interval >= 5)
                    {
                        interval *= 5;
                    }
                }
                if (_autoEvent.WaitOne(interval))
                {
                    //_logger.I(string.Format("{0} 任务执行了 {1} 次后被取消。", _mc.Name, i));
                    break;
                }
            }
            _autoEvent.Close();
            _autoEvent = null;
        }

        public override string ToString()
        {
            return _mission.GetType().FullName + " " + _mc;
        }
    }
}
