
using System;
using System.Threading.Tasks;
using Shoy.Utility.Helper;

namespace Shoy.Utility.Observer
{
    /// <summary>
    /// 观察者模式 发布者基类
    /// </summary>
    public abstract class PublisherBase<T>
    {
        public delegate void UpdateEventHandler(T sender);

        public event UpdateEventHandler Update;

        public string PublisherKey { get; private set; }

        protected PublisherBase()
        {
            PublisherKey = CombHelper.Guid16;
        }

        /// <summary> 订阅 消息通知 </summary>
        protected void Notify(T sender)
        {
            if (Update != null)
            {
                Update(sender);
            }
        }

        protected async void NotifyAsync(T sender)
        {
            if (Update != null)
            {
                await Task.Run(() => Update(sender));
            }
        }
    }
}
