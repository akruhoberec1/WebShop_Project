using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Technoshop.WebApi.Models.LoginResponse
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string Role { get; set; }    
        public Guid Id { get; set; }    
    }
}