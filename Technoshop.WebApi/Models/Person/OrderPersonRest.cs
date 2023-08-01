using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Technoshop.WebApi.Models.PersonRest
{
    public class OrderPersonRest
    {
        public Guid Id { get; set; }    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}