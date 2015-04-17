using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Shoy.Utility.Extend;

namespace Shoy.Utility.Observer
{
    /// <summary> 接收者 基类 一种行为</summary>
    public abstract class ObserverBase<T>
    {
        private readonly ConcurrentDictionary<string, PublisherBase<T>> _publishers;

        protected ObserverBase(params PublisherBase<T>[] publishers)
        {
            _publishers = new ConcurrentDictionary<string, PublisherBase<T>>();
            if (publishers != null && publishers.Any())
                AddPublishers(publishers);
        }

        /// <summary> 添加订阅者 </summary>
        /// <param name="key"></param>
        /// <param name="publisher">订阅者</param>
        public void AddPublisher(string key, PublisherBase<T> publisher)
        {
            if (_publishers.ContainsKey(key))
                return;
            publisher.Update += Response;
            _publishers.TryAdd(key, publisher);
        }

        /// <summary> 添加订阅者 </summary>
        /// <param name="publisher"></param>
        public void AddPublisher(PublisherBase<T> publisher)
        {
            AddPublisher(publisher.PublisherKey, publisher);
        }

        /// <summary> 批量添加订阅者 </summary>
        /// <param name="publishers"></param>
        public void AddPublishers(IEnumerable<PublisherBase<T>> publishers)
        {
            publishers.Each(AddPublisher);
        }

        /// <summary> 规划了观察者的一种行为(方法) </summary>
        protected abstract void Response(T sender);
    }
}
