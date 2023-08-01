using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Technoshop.WebApi.Models.AddressRest;
using Technoshop.WebApi.Models.Product;

namespace Technoshop.WebApi.Models.Order
{
    public class CreateOrderRest
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid BillingAddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductsTableListRest> ProductsRest { get; set; }
    }
}