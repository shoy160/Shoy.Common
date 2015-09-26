
namespace Shoy.Core.Events.EventData
{
    public class ChangedEventData<TEntity> : EventData
    {
        public TEntity Entity { get; private set; }

        public ChangedEventData(TEntity entity)
        {
            Entity = entity;
        }
    }
}
