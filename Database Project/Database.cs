using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Database_Project
{
    internal class Database
    {
        public Database()
        {
            SqlConnection cnn;
            string connectionString = $@"Data Source=DESKTOP-CDHTOA2;Initial Catalog = ASPAdventure; Integrated Security=SSPI;";
            cnn = new SqlConnection(connectionString);
            SqlDataReader OutPutSelectAll;
        }        
    }
}
