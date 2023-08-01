using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Model
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get;set; }    
        public string LastName { get;set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get;set; }
        public bool IsActive { get;set; }
        public Guid UpdatedBy { get; set; }
    }
}
