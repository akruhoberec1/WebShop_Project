using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Model
{
    public class Order
    {
        public Guid Id { get; set; }    
        public Guid PersonId { get; set; }  
        public decimal TotalPrice { get; set; }
        public Address ShippingAddress { get; set; }    
        public Address BillingAddress { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public Person Person { get; set; }  
        public List<Product> Products { get; set; }

    }
}
