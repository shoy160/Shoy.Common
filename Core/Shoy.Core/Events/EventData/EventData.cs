using System;
using Shoy.Utility.Timing;

namespace Shoy.Core.Events.EventData
{
    public abstract class EventData : IEventData
    {
        public DateTime EventTime { get; set; }
        public object EventSource { get; set; }

        protected EventData()
        {
            EventTime = Clock.Now;
        }
    }
}
