using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.Wiki.Models
{
    [Table("TWiki")]
    public class Wiki : DEntity<string>
    {
        [Key, StringLength(16)]
        public override string Id { get; set; }

        [Required, StringLength(32)]
        public string Name { get; set; }

        [Required, StringLength(16)]
        [ForeignKey("Group")]
        public string GroupId { get; set; }

        public string Description { get; set; }

        [Required, DefaultValue(0)]
        public int Hots { get; set; }

        [Required, DefaultValue(0)]
        public int Sort { get; set; }

        [Required, StringLength(16)]
        [ForeignKey("Creator")]
        public string CreatorId { get; set; }

        [Required, DefaultValue(0)]
        public int Status { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual WikiGroup Group { get; set; }
    }
}