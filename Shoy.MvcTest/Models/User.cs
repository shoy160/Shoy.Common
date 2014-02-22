using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoy.MvcTest.Models
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public DateTime CreateOn { get; set; }
    }
}