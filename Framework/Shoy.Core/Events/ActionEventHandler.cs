
using System;

namespace Shoy.Core.Events
{
    public class ActionEventHandler<TEventData>
        : IEventHandler<TEventData>
    {
        public Action<TEventData> Action { get; private set; }

        public ActionEventHandler(Action<TEventData> handler)
        {
            Action = handler;
        }

        public void HandleEvent(TEventData eventData)
        {
            Action(eventData);
        }
    }
}
