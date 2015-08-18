using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.MvcDemo.Models
{
    [Table("TU_Agency")]
    public class Agency : Entity<string>
    {
        [Column("AgencyID")]
        public override string Id { get; set; }
        public string AgencyName { get; set; }

        public virtual ICollection<MClass> Classes { get; set; }
    }
}