
using System;
using Shoy.Core.Events.EventData;

namespace Shoy.Core.Events
{
    /// <summary> 数据库事件管理器接口 </summary>
    public interface IEventsManager : ILifetimeDependency
    {
        /// <summary> 注册事件 </summary>
        /// <typeparam name="TEnventData"></typeparam>
        /// <param name="action"></param>
        IDisposable Register<TEnventData>(Action<TEnventData> action) where TEnventData : IEventData;

        /// <summary> 移除事件 </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="action"></param>
        void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        void Unregister(Type eventType, Type actionType);

        /// <summary> 触发事件 </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;

        void Trigger(Type eventType, object eventSource, IEventData eventData);
    }
}
