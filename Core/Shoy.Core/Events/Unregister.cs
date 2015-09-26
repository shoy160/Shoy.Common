using System;

namespace Shoy.Core.Events
{
    public class Unregister : IDisposable
    {
        private readonly IEventsManager _eventBus;
        private readonly Type _eventType;
        private readonly Type _actionType;

        public Unregister(IEventsManager eventBus, Type eventType, Type actionType)
        {
            _eventBus = eventBus;
            _eventType = eventType;
            _actionType = actionType;
        }

        public void Dispose()
        {
            _eventBus.Unregister(_eventType, _actionType);
        }
    }
}
