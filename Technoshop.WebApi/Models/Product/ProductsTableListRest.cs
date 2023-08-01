using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Technoshop.WebApi.Models.Product
{
    public class ProductsTableListRest
    {
        public Guid Id { get; set; }    
        public string Name { get; set; }    
        public decimal? Price { get; set; }  
        public int? Quantity { get; set; }   
    }
}