using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop
{
    public  class AddressFilter
    {
        public string SearchQuery { get; set; }
        public bool IsActive { get; set; }
        public Guid PersonId { get; set; }  
        public bool IsShipping { get; set; }    

    }
}
