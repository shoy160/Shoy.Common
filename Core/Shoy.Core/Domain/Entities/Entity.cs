using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shoy.Core.Domain.Entities
{
    /// <summary>
    /// 可持久化到数据库的数据模型基类
    /// </summary>
    public abstract class Entity : Entity<int> { }

    /// <summary>
    /// 可持久化到数据库的数据模型基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// 主键，唯一标识
        /// </summary>
        [Key]
        public virtual TKey Id { get; set; }

        public virtual bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
        }

//        /// <summary>
//        /// 版本控制标识，用于处理并发
//        /// </summary>
//        [ConcurrencyCheck]
//        [Timestamp]
//        public byte[] Timestamp { get; set; }

        /// <summary>
        /// 判断两个实体是否是同一数据记录的实体
        /// </summary>
        /// <param name="obj">要比较的实体信息</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey>))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (Entity<TKey>)obj;
            if (IsTransient() && other.IsTransient())
            {
                return false;
            }

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        /// 当前 <see cref="T:System.Object"/> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <inheritdoc/>
        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("[{0} {1}]", GetType().Name, Id);
        }
    }
}
