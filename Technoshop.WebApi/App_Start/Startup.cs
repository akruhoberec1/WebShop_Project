using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Technoshop.Repository;
using Technoshop.Service;
using Technoshop.Service.Common;
using Technoshop.Repository.Common;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Technoshop.WebApi.Auth;
using Microsoft.Extensions.Logging;

[assembly: OwinStartup(typeof(Technoshop.WebApi.App_Start.Startup))]

namespace Technoshop.WebApi.App_Start
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; set; }
        public void Configuration(IAppBuilder app)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<UserService>();
            var container = builder.Build();
            app.UseAutofacMiddleware(container);
          //  app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new AuthServerProvider(new UserRepository())
            };

            OAuthOptions = options;
            //Token Generators
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());


            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

        }
    }
}
