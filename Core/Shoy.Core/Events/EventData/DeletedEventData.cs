
namespace Shoy.Core.Events.EventData
{
    public class DeletedEventData<TEntity> : ChangedEventData<TEntity>
    {
        public DeletedEventData(TEntity entity)
            : base(entity)
        {
        }
    }
}
