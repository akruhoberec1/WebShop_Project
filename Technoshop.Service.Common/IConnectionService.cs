using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technoshop.Service.Common
{
    public interface IConnectionService
    {
        NpgsqlConnection OpenConnection();
        void CloseConnection(NpgsqlConnection connection);
    }
}
