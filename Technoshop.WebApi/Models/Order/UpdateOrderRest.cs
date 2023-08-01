using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Technoshop.WebApi.Models.Product;

namespace Technoshop.WebApi.Models.Order
{
    public class UpdateOrderRest
    {
        public Guid PersonId { get; set; }  
        public Guid ShippingAddressId { get; set; } 
        public Guid BillingAddressId { get; set; }  
        public decimal TotalPrice { get; set; } 
        public List<ProductsTableListRest> Products { get; set; }
    }
}