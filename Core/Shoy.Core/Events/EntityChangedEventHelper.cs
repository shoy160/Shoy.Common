using System;
using Shoy.Core.Events.EventData;

namespace Shoy.Core.Events
{
    public class EntityChangedEventHelper : IEntityChangedEventHelper
    {
        public IEventsManager EventsManager { get; set; }
        public void TriggerCreatedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(CreatedEventData<>), entity);
            TriggerEntityChangeEvent(typeof(ChangedEventData<>), entity);
        }

        public void TriggerUpdatedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(UpdatedEventData<>), entity);
            TriggerEntityChangeEvent(typeof(ChangedEventData<>), entity);
        }

        public void TriggerDeletedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(DeletedEventData<>), entity);
            TriggerEntityChangeEvent(typeof(ChangedEventData<>), entity);
        }

        private void TriggerEntityChangeEvent(Type genericEventType, object entity)
        {
            var entityType = entity.GetType();
            var eventType = genericEventType.MakeGenericType(entityType);
            //:todo 成功之后才触发~
            EventsManager.Trigger(eventType, null, (IEventData)Activator.CreateInstance(eventType, entity));
        }
    }
}
