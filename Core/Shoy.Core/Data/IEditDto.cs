
namespace Shoy.Core.Data
{
    /// <summary>
    /// 编辑DTO
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEditDto<TKey>
    {
        /// <summary>
        /// 主键，唯一标示
        /// </summary>
        TKey Id { get; set; }
    }
}
