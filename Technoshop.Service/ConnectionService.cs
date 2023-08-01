using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Service.Common;

namespace Technoshop.Service
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionStringProvider _connectionProvider; 

        public ConnectionService(IConnectionStringProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
    
        public NpgsqlConnection OpenConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            connection.Open();

            return connection;
        }

        public void CloseConnection(NpgsqlConnection connection) 
        { 
            connection.Close(); 
        }

    }
}
