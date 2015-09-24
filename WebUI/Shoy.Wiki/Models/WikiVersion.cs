using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.Wiki.Models
{
    [Table("TWikiVersion")]
    public class WikiVersion : DEntity<string>
    {
        [Key, StringLength(16)]
        public override string Id { get; set; }

        [Required, StringLength(32)]
        [ForeignKey("DWiki")]
        public string WikiId { get; set; }

        [Required, StringLength(32)]
        public string Version { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public virtual Wiki DWiki { get; set; }
    }
}