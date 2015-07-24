
namespace Shoy.Core.Domain.Entities
{
    /// <summary> 实体类接口 </summary>
    public interface IEntity : IEntity<int> { }

    /// <summary> 实体类接口 </summary>
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }

        bool IsTransient();
    }
}
