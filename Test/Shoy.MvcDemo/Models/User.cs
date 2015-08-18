using System;
using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.MvcDemo.Models
{
    [Table("TU_User")]
    public class User : Entity<long>
    {
        [Column("UserID")]
        public override long Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string TrueName { get; set; }

        public virtual DateTime AddedAt { get; set; }

        public byte Role { get; set; }
        public byte Status { get; set; }
    }
}