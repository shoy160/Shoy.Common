
namespace Shoy.Core.Domain.Entities
{
    /// <summary> 软删除接口 </summary>
    public interface ISoftDelete
    {
        /// <summary> 逻辑删除 </summary>
        bool IsDeleted { get; set; }
    }
}
