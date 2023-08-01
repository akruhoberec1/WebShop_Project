using System;

namespace Technoshop.Model
{
    public class Registration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public string Phone { get; set; }
        public string Username { get; set; }    
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } 
        public Guid RoleId { get; set; }    
    }
}
