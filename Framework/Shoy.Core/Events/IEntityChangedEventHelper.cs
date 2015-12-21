
namespace Shoy.Core.Events
{
    public interface IEntityChangedEventHelper : ILifetimeDependency
    {
        /// <summary> 执行创建事件 </summary>
        /// <param name="entity"></param>
        void TriggerCreatedEvent(object entity);

        /// <summary> 执行更新事件 </summary>
        /// <param name="entity"></param>
        void TriggerUpdatedEvent(object entity);

        /// <summary> 执行删除事件 </summary>
        /// <param name="entity"></param>
        void TriggerDeletedEvent(object entity);
    }
}
