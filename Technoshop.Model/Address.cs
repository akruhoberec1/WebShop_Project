using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Model
{
    public class Address
    {
        public Guid Id { get; set; }
        public string StreetName { get; set; }  
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public Guid PersonId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsShipping { get; set; }    
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

    }
}
