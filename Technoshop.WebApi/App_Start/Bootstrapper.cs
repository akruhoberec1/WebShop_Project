using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Technoshop.WebApi.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            IocConfig.Initialize(GlobalConfiguration.Configuration);
        }
    }
}