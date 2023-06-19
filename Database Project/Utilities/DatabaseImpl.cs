﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Database_Project.Entities;

namespace Database_Project.Utilities
{

    /// <summary>
    /// Implementation of the IDatabase interface.
    /// </summary>
    public class DatabaseImpl : IDatabase
    {
        private const string CONNECTION_STRING = $@"Data Source=DESKTOP-CDHTOA2;Initial Catalog = ASPAdventure; Integrated Security=SSPI;";

        public DatabaseImpl()
        {

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UserRegistration(string username, string password, string email)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

                command = new SqlCommand("INSERT INTO Utenti (Username, Password, Email) VALUES (@Username, @Password, @Email)", connection);
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

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Sellers WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    command = new SqlCommand("INSERT INTO Conti (IBAN, NomeDellaBanca, BIC_SWIFT) VALUES()");
                    command.Parameters.AddWithValue("@IBAN", IBAN);
                    command.Parameters.AddWithValue("@NomeDellaBanca", bankName);
                    command.Parameters.AddWithValue("@BIC_SWIFT", bicSwiftCode);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SELECT IDConto FROM Conti WHERE IBAN = @IBAN;");
                    command.Parameters.AddWithValue("@IBAN", IBAN);
                    SqlDataReader reader = command.ExecuteReader();
                    int accountId = -1;
                    while (reader.Read())
                    {
                        accountId = reader.GetInt32(0);
                    }

                    if (accountId < 0)
                    {
                        return false;
                    }

                    command = new SqlCommand("INSERT INTO Venditori (Username, Password, Email, Paese, IDConto) VALUES (@Username, @Password, @Email, @Paese, @IDConto)", connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Paese", country);
                    command.Parameters.AddWithValue("@IDConto", accountId);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }

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

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Admins WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

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

                return PasswordManager.CheckPassword(password, encryptedPassword);
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

                return PasswordManager.CheckPassword(password, encryptedPassword);
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

                return PasswordManager.CheckPassword(password, encryptedPassword);
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

                SqlCommand command = new SqlCommand("UPDATE Utenti SET Username = @Username, Email = @Email, " +
                    "Password = @Password, Via = @Street, NCivico = @CivicAddress, CAP = @CAP, Citta = @City, " +
                    "Paese = @Country, NumeroDiTelefono = @TelephoneNumber WHERE Username = @Username",
                    connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Street", user.Street);
                command.Parameters.AddWithValue("@CivicAddress", user.CivicAddress);
                command.Parameters.AddWithValue("@CAP", user.Cap);
                command.Parameters.AddWithValue("@City", user.City);
                command.Parameters.AddWithValue("@Country", user.Country);
                command.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
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

                SqlCommand command = new SqlCommand("INSERT INTO Offerte (UsernameVenditore, Prezzo, " +
                    "Quantita, Lingua, Locazione, Condizioni) VALUES (@Username, @Price, @Quantity, " +
                    "@Language, @Location, @Conditions)", connection);
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

                SqlCommand command = new SqlCommand("INSERT INTO Prodotti (Nome, Descrizione, Rarita, " +
                    "Gioco, Espansione) VALUES (@Name, @Description, @Rarity, @Game, @Expansion)", connection);
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

                SqlCommand command = new SqlCommand("UPDATE Prodotti SET Nome = @Name, " +
                    "Descrizione = @Description, Rarita = @Rarity, Gioco = @Game, Espansione = @Expansion " +
                    "WHERE IDProdotto = @ID", connection);
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

                SqlCommand command = new SqlCommand("INSERT INTO Espansioni (Name) VALUES (@Name)", connection);
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

                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command;

                try
                {
                    command = new SqlCommand("INSERT INTO Vendite (Data, Aquirente, Venditore, Feedback) OUTPUT IDVendita " +
                        "VALUES (@Date, @Buyer, @Seller, @Feedback)");
                    DateTime currDate = DateTime.Now;
                    command.Parameters.AddWithValue("@Date", currDate);
                    command.Parameters.AddWithValue("@Buyer", userUsername);
                    command.Parameters.AddWithValue("@Seller", sellerUsername);
                    command.Parameters.AddWithValue("@Feedback", feedback.FeedbackId);
                    int idVendita = (int)command.ExecuteScalar();

                    List<int> detailsIDs = new List<int>();

                    foreach (Detail detail in details)
                    {
                        command = new SqlCommand("INSERT INTO Dettagli (Prezzo, Quantita, IDOfferta, IDVendita) OUTPUT IDDettaglio" +
                        "VALUES (@Price, @Quantity, @OffertID, @SellID)", connection);
                        command.Parameters.AddWithValue("@Price", detail.Price);
                        command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                        command.Parameters.AddWithValue("@OffertID", detail.OffertId);
                        command.Parameters.AddWithValue("@SellID", idVendita);
                        detailsIDs.Add((int)command.ExecuteScalar());
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddToWishlist(string username, int productID, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Wishlists (Username, IDProdotto, Quantita) VALUES (@Username, @ProductID, @Quantity)", 
                    connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@Quantity", quantity);
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

                DateTime expireDate = DateTime.Now;
                expireDate = expireDate.AddYears(1);

                SqlCommand command = new SqlCommand("INSERT INTO Coupons (CodiceCoupon, DataDiScadenza, Valore, UsernameGeneratore) " +
                    "VALUES (@CouponCode, @ExpireDate, @Value, @Username)", connection);
                command.Parameters.AddWithValue("@CouponCode", RandomCoupon());
                command.Parameters.AddWithValue("@ExpireDate", expireDate);
                command.Parameters.AddWithValue("@Value", value);
                command.Parameters.AddWithValue("@Username", username);
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
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand command = new SqlCommand("INSERT INTO Conti (IBAN, NomeDellaBanca, BIC_SWIFT) " +
                        "VALUES (@IBAN, @BankName, @BicSwift)", connection);
                    command.Parameters.AddWithValue("@IBAN", IBAN);
                    command.Parameters.AddWithValue("@BankName", bankName);
                    command.Parameters.AddWithValue("@BicSwift", BicSwift);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SELECT IDConto FROM Conti WHERE IBAN = @IBAN;");
                    command.Parameters.AddWithValue("@IBAN", IBAN);
                    SqlDataReader reader = command.ExecuteReader();
                    int accountId = -1;
                    while (reader.Read())
                    {
                        accountId = reader.GetInt32(0);
                    }

                    if (accountId < 0)
                    {
                        throw new Exception("Couldn't find any reference to the bank account.");
                    }

                    command = new SqlCommand("UPDATE Utenti SET IDConto = @IDConto WHERE Username = @Username");
                    command.Parameters.AddWithValue("@IDConto", accountId);
                    command.Parameters.AddWithValue("@Username", username);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Method to generate coupons
        /// </summary>
        /// <returns>A new coupon code</returns>
        private string RandomCoupon()
        {
            Random random = new Random();            
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int couponLenght = 10;

            char[] couponCode = new char[couponLenght];

            for (int i = 0; i< couponLenght; i++)
            {
                couponCode[i] = chars[random.Next(chars.Length)];
            }

            return new string (couponCode);
        }
    }
}
