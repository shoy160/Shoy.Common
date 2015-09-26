
namespace Shoy.Core.Events.EventData
{
    public class CreatedEventData<TEntity> : ChangedEventData<TEntity>
    {
        public CreatedEventData(TEntity entity)
            : base(entity)
        {
        }
    }
}
