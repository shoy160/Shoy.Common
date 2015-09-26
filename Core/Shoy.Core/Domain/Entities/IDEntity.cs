
namespace Shoy.Core.Domain.Entities
{
    public interface IDEntity<TKey>
    {
        TKey Id { get; set; }

        bool IsTransient();

    }

    public interface IDEntity : IDEntity<int>
    { }
}
