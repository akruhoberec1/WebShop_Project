using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

using Technoshop.Model;
using Technoshop.Repository.Common;

namespace Technoshop.WebApi.Auth
{
    public class AuthServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUserRepository _userRepository;

        public AuthServerProvider(IUserRepository UserRepository)
        {
            _userRepository = UserRepository;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() =>
            {
                context.Validated();
            });
            
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            User request = new User() { UserName = context.UserName, Password = context.Password };


            User response = await _userRepository.ValidateUserAsync(request);

            if (response == null)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }
            Role role = await _userRepository.GetUserRoleAsync(response.Id);
            string userRole = role.Title;

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Role, userRole));
            identity.AddClaim(new Claim("Username", response.UserName));
            context.Validated(identity);
        }
    }
}