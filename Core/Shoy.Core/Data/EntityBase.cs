using System;
using System.ComponentModel.DataAnnotations;

namespace Shoy.Core.Data
{
    /// <summary>
    /// 可持久化到数据库的数据模型基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityBase<TKey>
    {
        protected EntityBase()
        {
            IsDelete = false;
            CreatedTime = DateTime.Now;
        }

        /// <summary>
        /// 主键，唯一标识
        /// </summary>
        [Key]
        public TKey Id { get; set; }

        /// <summary>
        /// 是否删除，逻辑删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 版本控制标识，用于处理并发
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] Timestamp { get; set; }

        /// <summary>
        /// 判断两个实体是否是同一数据记录的实体
        /// </summary>
        /// <param name="obj">要比较的实体信息</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var entity = obj as EntityBase<TKey>;
            if (entity == null)
            {
                return false;
            }
            return Id.Equals(entity.Id) && CreatedTime.Equals(entity.CreatedTime);
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        /// 当前 <see cref="T:System.Object"/> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ CreatedTime.GetHashCode();
        }
    }
}
