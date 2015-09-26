
using System;
using System.Collections.Generic;
using System.Reflection;
using Shoy.Core.Events.EventData;
using Shoy.Utility;

namespace Shoy.Core.Events
{
    public class EventsManager : IEventsManager
    {
        public EventsManager()
        {
            _handlers = new Dictionary<Type, List<IEventHandler>>();
        }

        public static EventsManager Instance
        {
            get
            {
                return (Singleton<EventsManager>.Instance ?? (Singleton<EventsManager>.Instance = new EventsManager()));
            }
        }

        private readonly Dictionary<Type, List<IEventHandler>> _handlers;

        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            lock (_handlers)
            {
                GetOrCreateHandlerFactories(typeof(TEventData)).Add(new ActionEventHandler<TEventData>(action));
                return new Unregister(this, typeof(TEventData), typeof(ActionEventHandler<TEventData>));
            }
        }

        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            lock (_handlers)
            {
                GetOrCreateHandlerFactories(typeof(TEventData))
                    .RemoveAll(t => t is ActionEventHandler<TEventData>);
            }
        }

        public void Unregister(Type eventType, Type actionType)
        {
            lock (_handlers)
            {
                GetOrCreateHandlerFactories(eventType)
                    .RemoveAll(t => t.GetType() == actionType);
            }
        }

        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            Trigger(typeof(TEventData), null, eventData);
        }

        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            eventData.EventSource = eventSource;
            lock (_handlers)
            {
                GetOrCreateHandlerFactories(eventType)
                    .ForEach(t =>
                    {
                        try
                        {
                            var actionType = typeof(ActionEventHandler<>).MakeGenericType(eventType);
                            var method = actionType.GetMethod("HandleEvent", BindingFlags.Public | BindingFlags.Instance,
                                null, new[] { eventType }, null);
                            method.Invoke(t, new object[] { eventData });
                        }
                        catch
                        {
                        }
                    });

            }
        }

        private List<IEventHandler> GetOrCreateHandlerFactories(Type eventType)
        {
            List<IEventHandler> handlers;
            if (!_handlers.TryGetValue(eventType, out handlers))
            {
                _handlers[eventType] = handlers = new List<IEventHandler>();
            }

            return handlers;
        }
    }
}
