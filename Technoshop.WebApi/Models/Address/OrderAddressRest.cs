using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Technoshop.WebApi.Models.AddressRest
{
    public class OrderAddressRest
    {
        public Guid Id { get; set; }    
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }    
        public string City { get; set; }
        public int Zipcode { get; set; }    

    }
}