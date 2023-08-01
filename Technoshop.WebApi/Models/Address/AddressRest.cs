using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Technoshop.WebApi.Models.Address
{
    public class AddressRest
    {
        public string StreetName { get; set;}
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public bool IsShipping { get; set; }    
    }
}