using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoy.Core.Domain.Entities;

namespace Shoy.CoreTest.Context.Models
{
    [Table("TU_User")]
    public class User : DEntity<long>
    {
        [Key]
        [Column("UserID")]
        public override long Id { get; set; }
        public virtual string Account { get; set; }
        public virtual string Email { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Password { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual byte PasswordType { get; set; }
        public virtual string Name { get; set; }
    }
}
