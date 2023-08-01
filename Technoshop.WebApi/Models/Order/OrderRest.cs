using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.WebApi.Models.AddressRest;
using Technoshop.WebApi.Models.Product;

namespace Technoshop.WebApi.Models
{
    public class OrderRest
    {
        public Guid Id { get; set; }    
        public Models.PersonRest.OrderPersonRest Person { get; set; }
        public OrderAddressRest ShippingAddress { get; set; }    
        public OrderAddressRest BillingAddress { get; set; }     
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductsTableListRest> ProductsRest { get; set; }  
    }
}