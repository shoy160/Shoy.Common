using System;

namespace Shoy.Core.Domain.Entities
{
    public interface IAudited : IAudited<long> { }
    public interface IAudited<TUserIdType>
    {
        DateTime CreationTime { get; set; }

        TUserIdType CreatorId { get; set; }

        string CreationIp { get; set; }

        DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user for this entity.
        /// </summary>
        TUserIdType LastModifierUserId { get; set; }
    }
}
