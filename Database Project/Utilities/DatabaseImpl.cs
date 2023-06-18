using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Database_Project.Entities;

namespace Database_Project.Utilities
{

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class DatabaseImpl: Database
    {
        private const string CONNECTION_STRING = $@"Data Source=DESKTOP-CDHTOA2;Initial Catalog = ASPAdventure; Integrated Security=SSPI;";
        
        private SqlConnection _sqlConnection;
        private PasswordManager _passwordManager;

        public DatabaseImpl()
        {
            _sqlConnection = new SqlConnection(CONNECTION_STRING);
            _passwordManager = new PasswordManager();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UserLogin(string username, string password)
        {
            _sqlConnection.Open();

            string sqlQuery = $"SELECT Password FROM Utenti WHERE Username == '{username}';";

            SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            string encryptedPassword = reader.GetString(2);
            _sqlConnection.Close();

            return _passwordManager.CheckPassword(password, encryptedPassword);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool SellerLogin(string username, string password)
        {
            _sqlConnection.Open();

            string sqlQuery = $"SELECT Password FROM Venditori WHERE Username == '{username}';";

            SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            string encryptedPassword = reader.GetString(2);
            _sqlConnection.Close();

            return _passwordManager.CheckPassword(password, encryptedPassword);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool AdminLogin(string username, string password)
        {
            _sqlConnection.Open();

            string sqlQuery = $"SELECT Password FROM Admin WHERE Username == '{username}';";

            SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            string encryptedPassword = reader.GetString(2);
            _sqlConnection.Close();

            return _passwordManager.CheckPassword(password, encryptedPassword);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<Product> GetProducts(string productName = "")
        {
            _sqlConnection.Open();

            string sqlQuery = $"SELECT * FROM Prodotti WHERE Nome LIKE '%{productName}%';";

            SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            List<Product> products = new List<Product>();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                DateTime date = reader.GetDateTime(2);
                string description = reader.GetString(3);
                string rarita = reader.GetString(4);
                string gioco = reader.GetString(5);
                string espansione = reader.GetString(6);

                Product product = new(id, name, date, description, rarita, gioco, espansione);

                products.Add(product);
            }

            reader.Close();

            return products;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<String> AskLogin (string tableName)
        {
            _sqlConnection.Open();

            // Retrieve column names from the specified table
            SqlCommand command = new SqlCommand (
                "SELECT COLUMN_NAME " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_NAME = @TableName", _sqlConnection);
            command.Parameters.AddWithValue("@TableName", tableName);

            List<string> lstColumn = new List<string>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                lstColumn.Add(reader.GetString(0));
            }

            reader.Close();
            _sqlConnection.Close();

            return lstColumn;
        }



    }
}
