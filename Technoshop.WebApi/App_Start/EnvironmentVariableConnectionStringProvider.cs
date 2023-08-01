using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Technoshop.Common;

namespace Technoshop.WebApi.App_Start
{
    public class EnvironmentVariableConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            return connectionString;
        }
    }
}