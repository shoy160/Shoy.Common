using System;

namespace Shoy.Core.Events.EventData
{
    public interface IEventData
    {
        DateTime EventTime { get; set; }

        object EventSource { get; set; }
    }
}
