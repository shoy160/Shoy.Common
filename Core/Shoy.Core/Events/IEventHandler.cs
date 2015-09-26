
namespace Shoy.Core.Events
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<in TEventData> : IEventHandler
    {
        void HandleEvent(TEventData eventData);
    }
}
