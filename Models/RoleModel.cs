using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAplication.Models
{
    public class RoleModel
    {
        [Key]
        public int ID { get; set; }
        public int UserId { get; set; }
        public virtual UserModel User { get; set; }
        public string RoleName { get; set; }

    }
}
