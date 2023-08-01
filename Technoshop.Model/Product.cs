using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Model.Common;

namespace Technoshop.Model
{
   public  class Product : IProduct
    {
        public Guid Id { get; set; }    
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string CategoryTitle { get; set; }   
        public int? Quantity { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}
        public bool? IsActive { get; set; }  
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set;}
        public Category Category { get; set; }  
        public List<Order> Orders { get; set; }
        public Person PersonCreated { get; set; }
        public Person PersonUpdated { get; set; }
    }
}
