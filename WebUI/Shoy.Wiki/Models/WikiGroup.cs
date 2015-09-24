using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.Wiki.Models
{
    [Table("TWikiGroup")]
    public class WikiGroup : DEntity<string>
    {
        [Key, StringLength(16)]
        public override string Id { get; set; }

        [Required, StringLength(64)]
        public string Group { get; set; }

        [Required,StringLength(64)]
        public string Code { get; set; }

        [StringLength(256)]
        public string Logo { get; set; }

        [Required, DefaultValue(0)]
        public int Sort { get; set; }

        [Required, DefaultValue(0)]
        public int WikiCount { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        [Required, StringLength(16)]
        [ForeignKey("Creator")]
        public string CreatorId { get; set; }

        public virtual User Creator { get; set; }
    }
}