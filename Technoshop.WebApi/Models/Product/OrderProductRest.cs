using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Technoshop.Model;
namespace Technoshop.WebApi.Models
{
    public class OrderProductRest
    {
        public Guid Id { get; set; }
        public string Name { get; set;}
        public decimal? Price { get; set; }  
        public int? Quantity { get; set; }
        public string CategoryTitle { get; set; }

        public Guid? CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Category Category { get; set; }
       // public List<Order> Orders { get; set; }
        public Person PersonCreated { get; set; }
        public Person PersonUpdated { get; set; }



    }
}