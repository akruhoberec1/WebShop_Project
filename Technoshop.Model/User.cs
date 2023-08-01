using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Model
{
    public class User : Person
    {
        public new Guid Id { get; set; }    
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public Guid PersonId { get; set; }
        public new DateTime CreatedAt { get; set; }
        public new DateTime UpdatedAt { get; set; }
        public new bool IsActive { get; set; } 
        public new Guid UpdatedBy { get; set; }
        public Person Person { get; set; }  
        public Role Role { get; set; }
    }
}
