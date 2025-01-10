using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    internal static class DBConnection
    {
       internal static SqlConnection GetConnection()
        {
            SqlConnection conn = null;
            string connectionString = @"Data Source=localhost;Initial Catalog=library_db;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            
            conn = new SqlConnection(connectionString);
            return conn;
        }
    }
}
