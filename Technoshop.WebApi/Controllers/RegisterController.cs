using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Technoshop.Model;
using Technoshop.Service;
using Technoshop.Service.Common;
using Technoshop.WebApi.App_Start;
using Technoshop.WebApi.Models.LoginResponse;


namespace Technoshop.WebApi.Controllers
{
    [RoutePrefix("api/register")]
    public class RegisterController : ApiController
    {
        private readonly IUserService _userService;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<HttpResponseMessage> Register([FromBody]Registration data)
        {
            User user = await _userService.RegisterUser(data);
            return (user != null) ? Request.CreateResponse(HttpStatusCode.OK, user) : Request.CreateResponse(HttpStatusCode.BadRequest,"Something went wrong");  
        }

        [HttpPost]
        [Route("register/admin")]
        public async Task<HttpResponseMessage> RegisterAdmin([FromBody] Registration data)
        {
            User user = await _userService.RegisterAdmin(data);
            return (user != null) ? Request.CreateResponse(HttpStatusCode.OK, user) : Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong");
        }

        [HttpPost]
        [Route("login")]
        public async Task<HttpResponseMessage> Login([FromBody] LoginData data)
        {
            try
            {
                User user = await _userService.LoginUserAsync(data);

                if (user != null)
                {
                    var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.Title));
                    identity.AddClaim(new Claim("Username", user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    // Create an authentication ticket with the identity
                    var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
                    // Generate the access token
                    var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
                    // Return the access token with user roles

                    LoginResponse response = new LoginResponse
                    {
                        AccessToken = accessToken,
                        Role = user.Role.Title,
                        Id = user.Id
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid username or password.");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex);
            }

        }

        [HttpGet]
        [Route("logout")]
        public HttpResponseMessage Logout()
        {
            Request.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);

            return Request.CreateResponse(HttpStatusCode.OK, "Logged out successfully.");
        }







    }
}
