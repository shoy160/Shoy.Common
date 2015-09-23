
namespace Shoy.Core.Events.EventData
{
    public class UpdatedEventData<TEntity> : ChangedEventData<TEntity>
    {
        public UpdatedEventData(TEntity entity)
            : base(entity)
        {
        }
    }
}
