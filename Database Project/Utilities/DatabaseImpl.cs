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
    public class DatabaseImpl : IDatabase
    {
        private const string CONNECTION_STRING = $@"Data Source=DESKTOP-CDHTOA2;Initial Catalog = ASPAdventure; Integrated Security=SSPI;";

        private PasswordManager _passwordManager;

        public DatabaseImpl()
        {
            _passwordManager = new PasswordManager();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<String> AskLogin(string tableName)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Retrieve column names from the specified table
                SqlCommand command = new SqlCommand(
                "SELECT COLUMN_NAME " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_NAME = @TableName", connection);
                command.Parameters.AddWithValue("@TableName", tableName);

                List<string> lstColumn = new List<string>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstColumn.Add(reader.GetString(0));
                }

                reader.Close();

                return lstColumn;
            }

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UserRegistration(string username, string password, string email)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Check if the username or email already exists in the database
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false; // Username or email already exists
                }

                // Insert the new user into the database
                command = new SqlCommand("INSERT INTO Users (Username, Password, Email) VALUES (@Username, @Password, @Email)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();

                return true;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool SellerRegistration(string username, string password, string email, string country, string IBAN, string bankName, string bicSwiftCode)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Check if the username or email already exists in the database
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Sellers WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false; // Username or email already exists
                }

                // Insert the new seller into the database
                command = new SqlCommand("INSERT INTO Sellers (Username, Password, Email, Country, IBAN, BankName, BicSwiftCode) VALUES (@Username, @Password, @Email, @Country, @IBAN, @BankName, @BicSwiftCode)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Country", country);
                command.Parameters.AddWithValue("@IBAN", IBAN);
                command.Parameters.AddWithValue("@BankName", bankName);
                command.Parameters.AddWithValue("@BicSwiftCode", bicSwiftCode);
                command.ExecuteNonQuery();

                return true;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool AdminRegistration(string username, string password, string email)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Check if the username or email already exists in the database
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Admins WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false; // Username or email already exists
                }

                // Insert the new admin into the database
                command = new SqlCommand("INSERT INTO Admins (Username, Password, Email) VALUES (@Username, @Password, @Email)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();

                return true;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UserLogin(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sqlQuery = $"SELECT Password FROM Utenti WHERE Username == '{username}';";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                string encryptedPassword = reader.GetString(2);

                return _passwordManager.CheckPassword(password, encryptedPassword);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool SellerLogin(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sqlQuery = $"SELECT Password FROM Venditori WHERE Username == '{username}';";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                string encryptedPassword = reader.GetString(2);

                return _passwordManager.CheckPassword(password, encryptedPassword);
            }
        }

        // CHECK ADMIN EXPIRE DATE

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool AdminLogin(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sqlQuery = $"SELECT Password FROM Admin WHERE Username == '{username}';";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                string encryptedPassword = reader.GetString(2);

                return _passwordManager.CheckPassword(password, encryptedPassword);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void EditUserProfile(User user)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Update the user profile in the Users table
                SqlCommand command = new SqlCommand("UPDATE Users SET Email = @Email, FullName = @FullName, Address = @Address WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<Product> GetProducts(string productName = "")
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sqlQuery = $"SELECT * FROM Prodotti WHERE Nome LIKE '%{productName}%';";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
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
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddOffert(string username, float price, int quantity, string language, string location, string conditions)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Insert the new offer into the Offers table
                SqlCommand command = new SqlCommand("INSERT INTO Offers (Username, Price, Quantity, Language, Location, Conditions) VALUES (@Username, @Price, @Quantity, @Language, @Location, @Conditions)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Language", language);
                command.Parameters.AddWithValue("@Location", location);
                command.Parameters.AddWithValue("@Conditions", conditions);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddProduct(string name, string description, string rarity, string game, string expansion)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Insert the new product into the Products table
                SqlCommand command = new SqlCommand("INSERT INTO Products (Name, Description, Rarity, Game, Expansion) VALUES (@Name, @Description, @Rarity, @Game, @Expansion)", connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Rarity", rarity);
                command.Parameters.AddWithValue("@Game", game);
                command.Parameters.AddWithValue("@Expansion", expansion);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void EditProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Update the product information in the Products table
                SqlCommand command = new SqlCommand("UPDATE Products SET Name = @Name, Description = @Description, Rarity = @Rarity, Game = @Game, Expansion = @Expansion WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", product.ProductId);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Rarity", product.Rarity);
                command.Parameters.AddWithValue("@Game", product.Game);
                command.Parameters.AddWithValue("@Expansion", product.Expansion);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddExpansion(string name)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Insert the new expansion into the Expansions table
                SqlCommand command = new SqlCommand("INSERT INTO Expansions (Name) VALUES (@Name)", connection);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Buy(string userUsername, string sellerUsername, Feedback feedback, List<Detail> details, List<string> coupons)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Perform the buy operation logic here
                // You can handle database transactions and update multiple tables as necessary
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddToWishlist(string username, int productID)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Insert the product into the user's wishlist in the Wishlists table
                SqlCommand command = new SqlCommand("INSERT INTO Wishlists (Username, ProductID) VALUES (@Username, @ProductID)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void CreateCoupon(string username, int value)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Create a new coupon for the user in the Coupons table
                SqlCommand command = new SqlCommand("INSERT INTO Coupons (Username, Value) VALUES (@Username, @Value)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Value", value);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddUserBankAccount(string username, string IBAN, string bankName, string BicSwift)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                // Insert the user's bank account into the UserBankAccounts table
                SqlCommand command = new SqlCommand("INSERT INTO UserBankAccounts (Username, IBAN, BankName, BicSwift) VALUES (@Username, @IBAN, @BankName, @BicSwift)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@IBAN", IBAN);
                command.Parameters.AddWithValue("@BankName", bankName);
                command.Parameters.AddWithValue("@BicSwift", BicSwift);
                command.ExecuteNonQuery();

            }
        }
    }
}
