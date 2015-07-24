namespace Shoy.Core.Domain.Entities
{
    public static class EntityExtensions
    {
        public static bool IsNullOrDeleted(this ISoftDelete entity)
        {
            return entity == null || entity.IsDeleted;
        }
    }
}