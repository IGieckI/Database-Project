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
    /// Implementation of the IDatabase interface.
    /// </summary>
    public class DatabaseImpl : IDatabase
    {
        private const string CONNECTION_STRING = $@"Data Source=DESKTOP-CDHTOA2;Initial Catalog = CardMarket; Integrated Security=SSPI;";

        public DatabaseImpl()
        {

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool UserRegistration(Account account, BankAccount bankAccount = null)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Utenti WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", account.Username);
                command.Parameters.AddWithValue("@Email", account.Email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

                if (bankAccount != null)
                {

                }

                command = new SqlCommand("INSERT INTO Utenti (Username, Password, Email, Via, NCivico, CAP, Citta, Paese, NumeroDiTelefono" + (bankAccount is null ? "" : ", IDConto") + ") VALUES (@Username, @Password, @Email, @Street, @CivicNumber, @CAP, @City, @Country, @TelephoneNumber" + (bankAccount is null ? "" : ", @BankAccountID") + ")", connection);
                command.Parameters.AddWithValue("@Username", account.Username);
                command.Parameters.AddWithValue("@Password", PasswordManager.EncryptPassword(account.Password));
                command.Parameters.AddWithValue("@Email", account.Email);
                command.Parameters.AddWithValue("@Street", account.Street);
                command.Parameters.AddWithValue("@CivicNumber", account.CivicNumber is null ? DBNull.Value : account.CivicNumber);
                command.Parameters.AddWithValue("@CAP", account.Cap is null ? DBNull.Value : account.Cap);
                command.Parameters.AddWithValue("@City", account.City);
                command.Parameters.AddWithValue("@Country", account.Country);
                command.Parameters.AddWithValue("@TelephoneNumber", account.TelephoneNumber is null ? DBNull.Value : account.TelephoneNumber);
                command.Parameters.AddWithValue("@BankAccountID", bankAccount is null ? DBNull.Value : bankAccount.BankAccountID );
                command.ExecuteNonQuery();

                return true;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool SellerRegistration(Account account, BankAccount bankAccount)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Venditori WHERE Username = @Username OR Email = @Email;", connection);
                command.Parameters.AddWithValue("@Username", account.Username);
                command.Parameters.AddWithValue("@Email", account.Email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    command = new SqlCommand("INSERT INTO Conti (IBAN, NomeDellaBanca, BIC_SWIFT) VALUES(@IBAN, @BankName, @BIC_SWIFT);", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@IBAN", bankAccount.IBAN);
                    command.Parameters.AddWithValue("@BankName", bankAccount.BankName);
                    command.Parameters.AddWithValue("@BIC_SWIFT", bankAccount.BIC_SWIFT);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SELECT IDConto FROM Conti WHERE IBAN = @IBAN;", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@IBAN", bankAccount.IBAN);
                    SqlDataReader reader = command.ExecuteReader();
                    int accountId = -1;
                    while (reader.Read())
                    {
                        accountId = reader.GetInt32(0);
                    }

                    reader.Close();

                    if (accountId < 0)
                    {
                        return false;
                    }

                    command = new SqlCommand("INSERT INTO Venditori (Username, Password, Email, Paese, IDConto) VALUES (@Username, @Password, @Email, @Country, @BankAccountID);", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@Username", account.Username);
                    command.Parameters.AddWithValue("@Password", PasswordManager.EncryptPassword(account.Password));
                    command.Parameters.AddWithValue("@Email", account.Email);
                    command.Parameters.AddWithValue("@Country", account.Country);
                    command.Parameters.AddWithValue("@BankAccountID", accountId);
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
        public bool AdminRegistration(Account account)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Admin WHERE Username = @Username OR Email = @Email", connection);
                command.Parameters.AddWithValue("@Username", account.Username);
                command.Parameters.AddWithValue("@Email", account.Email);
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    return false;
                }

                command = new SqlCommand("INSERT INTO Admin (Username, Password, Email) VALUES (@Username, @Password, @Email)", connection);
                command.Parameters.AddWithValue("@Username", account.Username);
                command.Parameters.AddWithValue("@Password", PasswordManager.EncryptPassword(account.Password));
                command.Parameters.AddWithValue("@Email", account.Email);
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

                SqlCommand command = new SqlCommand("SELECT * FROM Utenti WHERE Username = @Username;", connection);
                command.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return false;
                }

                reader.Read();
                string encryptedPassword = reader.GetString(2);
                reader.Close();

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

                SqlCommand command = new SqlCommand("SELECT * FROM Venditori WHERE Username = @Username;", connection);
                command.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return false;
                }

                reader.Read();
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

                SqlCommand command = new SqlCommand("SELECT * FROM Admin WHERE Username = @Username;", connection);
                command.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return false;
                }

                reader.Read();
                string encryptedPassword = reader.GetString(2);

                return PasswordManager.CheckPassword(password, encryptedPassword);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void EditUserProfile(Account user)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UPDATE Utenti SET Username = @Username, Email = @Email, " +
                    "Password = @Password, Via = @Street, NCivico = @CivicNumber, CAP = @CAP, Citta = @City, " +
                    "Paese = @Country, NumeroDiTelefono = @TelephoneNumber WHERE Username = @Username",
                    connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Street", user.Street);
                command.Parameters.AddWithValue("@CivicNumber", user.CivicNumber is null ? DBNull.Value : user.CivicNumber);
                command.Parameters.AddWithValue("@CAP", user.Cap is null ? DBNull.Value : user.Cap);
                command.Parameters.AddWithValue("@City", user.City);
                command.Parameters.AddWithValue("@Country", user.Country);
                command.Parameters.AddWithValue("@TelephoneNumber", user.TelephoneNumber);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<Product> GetProducts(string productName = "", string rarity = "", string game = "")
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                string sqlQuery = $"SELECT * FROM Prodotti WHERE Nome LIKE '%' + @ProductName + '%'";

                if (rarity != "")
                    sqlQuery += " AND Rarita = @Rarity";

                if (game != "")
                    sqlQuery += " AND Gioco = @Game";

                sqlQuery += ";";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@Rarity", rarity);
                command.Parameters.AddWithValue("@Game", game);

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
                    string espansione = reader.GetValue(6) is DBNull ? "" : reader.GetString(6);

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
        public Product? GetProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Prodotti WHERE IDProdotto = @ProductId;", connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();

                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                DateTime date = reader.GetDateTime(2);
                string description = reader.GetString(3);
                string rarita = reader.GetString(4);
                string gioco = reader.GetString(5);
                string espansione = reader.GetValue(6) is DBNull ? "" : reader.GetString(6);

                reader.Close();

                return new Product(id, name, date, description, rarita, gioco, espansione);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<string> GetRarities()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Rarita;", connection);
                SqlDataReader reader = command.ExecuteReader();

                var rarities = new List<string>();

                while (reader.Read())
                {
                    rarities.Add(reader.GetString(0));
                }

                reader.Close();

                return rarities;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<string> GetGames()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Giochi;", connection);
                SqlDataReader reader = command.ExecuteReader();

                var games = new List<string>();

                while (reader.Read())
                {
                    games.Add(reader.GetString(0));
                }

                reader.Close();

                return games;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<string> GetExpansions()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();;

                SqlCommand command = new SqlCommand("SELECT * FROM Espansioni;", connection);
                SqlDataReader reader = command.ExecuteReader();

                var games = new List<string>();

                while (reader.Read())
                {
                    games.Add(reader.GetString(0));
                }

                reader.Close();

                return games;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<string> GetConditions()
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open(); ;

                SqlCommand command = new SqlCommand("SELECT * FROM Condizioni;", connection);
                SqlDataReader reader = command.ExecuteReader();

                var games = new List<string>();

                while (reader.Read())
                {
                    games.Add(reader.GetString(0));
                }

                reader.Close();

                return games;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Coupon? GetCoupon(string couponCode)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open(); ;

                SqlCommand command = new SqlCommand("SELECT * FROM Coupon;", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                    return null;

                reader.Read();

                DateTime date = reader.GetDateTime(1);
                decimal value = reader.GetDecimal(2);
                string userGenerator = reader.GetValue(3) is null ? "" : reader.GetString(3);
                string userUtilizer = reader.GetValue(4) is null ? "" : reader.GetString(4);

                reader.Close();

                return new Coupon(couponCode, date, value, userGenerator, userUtilizer);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddOffert(string seller, float price, int quantity, string language, string location, string conditions, int productId)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Offerte (Prezzo, Quantita, Lingua, Locazione, Condizione, Prodotto, UsernameVenditore)" +
                    " VALUES (@Price, @Quantity, @Language, @Location, @Conditions, @Product, @Seller)", connection);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Language", language);
                command.Parameters.AddWithValue("@Location", location);
                command.Parameters.AddWithValue("@Conditions", conditions);
                command.Parameters.AddWithValue("@Product", productId);
                command.Parameters.AddWithValue("@Seller", seller);
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

                SqlCommand command = new SqlCommand("INSERT INTO Espansioni (Nome) VALUES (@Name)", connection);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddCondition(string name)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Condizioni (Nome) VALUES (@Name)", connection);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddRarity(string name)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Rarita (Nome) VALUES (@Name)", connection);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddGame(string name)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Giochi (Nome) VALUES (@Name)", connection);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Buy(string userUsername, Feedback feedback, List<Detail> details, List<Coupon> coupons)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command;

                try
                {
                    command = new SqlCommand("INSERT INTO Vendite (Aquirente, Feedback) OUTPUT IDVendita " +
                        "VALUES (@Buyer, @Feedback);", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@Buyer", userUsername);
                    command.Parameters.AddWithValue("@Feedback", feedback.FeedbackId);
                    int idVendita = (int)command.ExecuteScalar();

                    List<int> detailsIDs = new List<int>();
                    decimal totalCost = 0;

                    foreach (Detail detail in details)
                    {
                        command = new SqlCommand("INSERT INTO Dettagli (Prezzo, Quantita, IDOfferta, IDVendita) OUTPUT IDDettaglio" +
                        "VALUES (@Price, @Quantity, @OffertID, @SellID);", connection);
                        command.Transaction = transaction;
                        command.Parameters.AddWithValue("@Price", detail.Price);
                        command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                        command.Parameters.AddWithValue("@OffertID", detail.OffertId);
                        command.Parameters.AddWithValue("@SellID", idVendita);
                        detailsIDs.Add((int)command.ExecuteScalar());

                        totalCost += detail.Price * detail.Quantity;
                    }

                    foreach (Coupon coupon in coupons)
                    {
                        totalCost -= coupon.Value;
                    }

                    command = new SqlCommand("UPDATE Utenti SET Conto = Conto - @TotalCost;", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@TotalCost", totalCost);
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
        /// <inheritdoc/>
        /// </summary>
        public void AddToWishlist(string username, int productID, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Wishlist WHERE Username = @Username AND IDProdotto = @ProductID;", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@ProductID", productID);

                SqlDataReader reader = command.ExecuteReader();                
                reader.Read();

                int curQuantity = reader.HasRows ? reader.GetInt32(2) : -1;

                reader.Close();

                if (curQuantity == -1)
                {
                    command = new SqlCommand("INSERT INTO Wishlist (Username, IDProdotto, Quantita) VALUES (@Username, @ProductID, @Quantity)", connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ProductID", productID);
                    command.Parameters.AddWithValue("@Quantity", 1);
                }
                else
                {
                    command = new SqlCommand("UPDATE Wishlist SET Quantita = @NewQuantity WHERE Username = @Username AND IDProdotto = @ProductID;", connection);
                    command.Parameters.AddWithValue("@NewQuantity", curQuantity == -1 ? 1 : ++curQuantity);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ProductID", productID);
                }

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<WishlistItem> GetWishlist(string username)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Wishlist WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = command.ExecuteReader();

                var offertsId = new List<Tuple<int,int>>();
                var offerts = new List<WishlistItem>();

                while (reader.Read())
                {
                    offertsId.Add(new Tuple<int,int>(reader.GetInt32(1), reader.GetInt32(2)));
                }

                reader.Close();

                foreach(var offert in offertsId)
                {
                    command = new SqlCommand("SELECT * FROM Prodotti WHERE IDProdotto = @ProductID;", connection);
                    command.Parameters.AddWithValue("@ProductID", offert.Item1);
                    
                    reader = command.ExecuteReader();
                    reader.Read();

                    offerts.Add(new WishlistItem(reader.GetString(1), offert.Item2, reader.GetString(4), reader.GetString(5), reader.GetValue(6) == DBNull.Value ? "" : reader.GetString(6)));

                    reader.Close();
                }

                return offerts;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<Offert> GetOfferts(int productId)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Offerte WHERE Prodotto = @ProductID;", connection);
                command.Parameters.AddWithValue("@ProductID", productId);
                SqlDataReader reader = command.ExecuteReader();

                var offerts = new List<Offert>();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    decimal price = reader.GetDecimal(1);
                    int quantity = reader.GetInt32(2);
                    string language = reader.GetString(3);
                    string location = reader.GetString(4);
                    string condition = reader.GetString(5);
                    string sellerUsername = reader.GetString(7);

                    offerts.Add(new Offert(id, price, quantity, condition, language, location, productId, sellerUsername));
                }

                reader.Close();

                return offerts;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool AddToCart(int offertId)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {

                    SqlCommand command = new SqlCommand("SELECT * FROM Offerte WHERE IDOfferta = @OffertId;", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@OffertId", offertId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return false;
                    }

                    reader.Read();

                    int id = reader.GetInt32(0);
                    decimal price = reader.GetDecimal(1);
                    int quantity = reader.GetInt32(2);
                    string language = reader.GetString(3);
                    string location = reader.GetString(4);
                    string condition = reader.GetString(5);
                    string sellerUsername = reader.GetString(7);
                    
                    reader.Close();

                    if (quantity == 1)
                    {
                        command = new SqlCommand("DELETE FROM Offerte WHERE IDOfferta = @OffertId;", connection);
                        command.Transaction = transaction;
                        command.Parameters.AddWithValue("@OffertId", offertId);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command = new SqlCommand("UPDATE Offerte SET Quantita = Quantita - 1 WHERE IDOfferta = @OffertId;", connection);
                        command.Transaction = transaction;
                        command.Parameters.AddWithValue("@OffertId", offertId);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch(Exception ex)
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
        public Account GetUserAccount(string username)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Utenti WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new Exception("Account not found");
                }

                reader.Read();

                string email = reader.GetString(1);
                string password = reader.GetString(2);
                string street = reader.GetValue(3) is DBNull ? "" : reader.GetString(3);
                int? civicNumber = reader.GetValue(4) is DBNull ? null : reader.GetInt32(4);
                int? cap = reader.GetValue(5) is DBNull ? null : reader.GetInt32(5);
                string city = reader.GetValue(6) is DBNull ? "" : reader.GetString(6);
                string country = reader.GetValue(7) is DBNull ? "" : reader.GetString(7);
                string telephoneNumber = reader.GetValue(8) is DBNull ? "" : reader.GetString(8);
                int credit = reader.GetInt32(9);
                int? bankAccountID = reader.GetValue(10) is DBNull ? null : reader.GetInt32(10);

                reader.Close();

                return new Account(username, email, password, street, civicNumber, cap, city, country, telephoneNumber, credit, bankAccountID);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BankAccount GetBankAccount(int bankAccountId)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Conti WHERE IDConto = @BankAccountId", connection);
                command.Parameters.AddWithValue("@BankAccountId", bankAccountId);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new Exception("Bank account not found");
                }

                reader.Read();

                string IBAN = reader.GetString(1);
                string bankName = reader.GetString(2);
                string bicSwift = reader.GetString(3);

                reader.Close();

                BankAccount bankAccount = new(bankAccountId, IBAN, bankName, bicSwift);


                return bankAccount;
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
        public bool CheckCoupon(string couponCode)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) Coupon WHERE CodiceCoupon = @CouponCode AND DataDiScadenza <= GETDATE() AND UsernameUtilizzatore IS NULL;", connection);
                command.Parameters.AddWithValue("@CouponCode", couponCode);
                if ((int)command.ExecuteScalar() > 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void UseCoupon(string couponCode, string user)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UPDATE Coupon SET UsernameUtilizzatore = @User WHERE CodiceCoupon = @CouponCode;", connection);
                command.Parameters.AddWithValue("@CouponCode", couponCode);
                command.Parameters.AddWithValue("@User", user);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void AddUserBankAccount(string username, BankAccount bankAccount)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand command = new SqlCommand("INSERT INTO Conti (IBAN, NomeDellaBanca, BIC_SWIFT) VALUES (@IBAN, @BankName, @BicSwift);", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@IBAN", bankAccount.IBAN);
                    command.Parameters.AddWithValue("@BankName", bankAccount.BankName);
                    command.Parameters.AddWithValue("@BicSwift", bankAccount.BIC_SWIFT);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("SELECT IDConto FROM Conti WHERE IBAN = @IBAN;", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@IBAN", bankAccount.IBAN);
                    SqlDataReader reader = command.ExecuteReader();
                    int accountId = -1;
                    while (reader.Read())
                    {
                        accountId = reader.GetInt32(0);
                    }

                    reader.Close();

                    if (accountId < 0)
                    {
                        throw new Exception("Couldn't find any reference to the bank account.");
                    }

                    command = new SqlCommand("UPDATE Utenti SET IDConto = @IDConto WHERE Username = @Username;", connection);
                    command.Transaction = transaction;
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
        /// <inheritdoc/>
        /// </summary>
        public void UpdateUserBankAccount(BankAccount bankAccount)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UPDATE Conti SET IBAN = @IBAN, NomeDellaBanca = @BankName, BIC_SWIFT = @BicSwift " +
                        "WHERE IDConto = @BankAccountID", connection);
                command.Parameters.AddWithValue("@IBAN", bankAccount.IBAN);
                command.Parameters.AddWithValue("@BankName", bankAccount.BankName);
                command.Parameters.AddWithValue("@BicSwift", bankAccount.BIC_SWIFT);
                command.Parameters.AddWithValue("@BankAccountID", bankAccount.BankAccountID);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void RemoveUserBankAccount(Account account)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand command = new SqlCommand("UPDATE Utenti SET IDConto = NULL WHERE Username = @Username;", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@Username", account.Username);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("DELETE FROM Conti WHERE IDConto = @BankAccountId;", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@BankAccountId", account.BankAccountID);
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
