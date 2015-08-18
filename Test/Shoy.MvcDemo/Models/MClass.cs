using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.MvcDemo.Models
{
    [Table("TU_Class")]
    public class MClass : Entity<string>
    {
        [Column("ClassID")]
        public override string Id { get; set; }

        public virtual string ClassName { get; set; }

        public virtual long? ClassManagerID { get; set; }

        [ForeignKey("ClassManagerID")]
        public virtual User Manager { get; set; }

        public string AgencyID { get; set; }

        [ForeignKey("AgencyID")]
        public virtual Agency Agency { get; set; }
        public virtual byte Status { get; set; }

        public virtual string ClassCode { get; set; }
    }
}