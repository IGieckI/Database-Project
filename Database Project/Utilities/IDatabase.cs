using Database_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Utilities
{
     /// <summary>
     /// Class which provide a connection to the database 
     /// </summary>
    internal interface IDatabase
    {
        /// <summary>
        /// Register a new User.
        /// </summary>
        /// <param name="account">Object containing user's informations</param>
        /// <param name="bankAccount">(Optional) Object containing informations related to the bank account of the user</param>
        /// <returns>True if registered correctly, false otherwise</returns>
        bool UserRegistration(Account account, BankAccount bankAccount = null);

        /// <summary>
        /// Register a new Seller.
        /// </summary>
        /// <param name="account">Object containing seller's informations</param>
        /// <param name="bankAccount">Object containing informations related to the bank account of the seller</param>
        /// <returns>True if registered correctly, false otherwise</returns>
        bool SellerRegistration(Account account, BankAccount bankAccount);

        /// <summary>
        /// Register a new Admin.
        /// </summary>
        /// <param name="account">Object containing seller's informations</param>
        /// <returns>True if registered correctly, false otherwise</returns>
        bool AdminRegistration(Account account);

        /// <summary>
        /// Check if the credentials are correct for an User
        /// </summary>
        /// <param name="username">Inserted username</param>
        /// <param name="password">Inserted password</param>
        /// <returns> true if log in successfully, false otherwise </returns>
        bool UserLogin(string username, string password);

        /// <summary>
        /// Check if the credentials are correct for a Seller
        /// </summary>
        /// <param name="username">Inserted username</param>
        /// <param name="password">Inserted password</param>
        /// <returns> true if log in successfully, false otherwise </returns>
        bool SellerLogin(string username, string password);

        /// <summary>
        /// Check if the credentials are correct for an Admin
        /// </summary>
        /// <param name="username">Inserted username</param>
        /// <param name="password">Inserted password</param>
        /// <returns> true if log in successfully, false otherwise </returns>
        bool AdminLogin(string username, string password);

        /// <summary>
        /// Edit a user profile
        /// </summary>
        /// <param name="user">New user profile settings</param>
        void EditUserProfile(Account user);

        /// <summary>
        /// Retrive from database all the information about the product asked for.
        /// </summary>
        /// <param name="productName"> The entire or a part of the product name</param>
        /// <param name="game"> The game filter</param>
        /// <param name="rarity"> The rarity filter</param>
        /// <returns>A List of Product contained in the database</returns>
        List<Product> GetProducts(string productName = "", string rarity = "", string game = "");

        /// <summary>
        /// Retrive every rarity from the database
        /// </summary>
        /// <returns>A list of rarities names</returns>
        List<string> GetRarities();

        /// <summary>
        /// Retrive every game from the database
        /// </summary>
        /// <returns>A list the games names</returns>
        List<string> GetGames();

        /// <summary>
        /// Retrive every expansion from the database
        /// </summary>
        /// <returns>A list the expansion names</returns>
        List<string> GetExpansions();

        /// <summary>
        /// Retrive every condition from the database
        /// </summary>
        /// <returns>A list the conditions names</returns>
        List<string> GetConditions();

        /// <summary>
        /// A Seller adding an offert.
        /// </summary>
        /// <param name="username">Seller's username</param>
        /// <param name="price">Single product's price</param>
        /// <param name="quantity">Quantity of product</param>
        /// <param name="language">Language of product</param>
        /// <param name="location">Product location</param>
        /// <param name="conditions">Product conditions</param>
        void AddOffert(string seller, float price, int quantity, string language, string location, string conditions, int productId);

        /// <summary>
        /// Add a new type of product to the database.
        /// </summary>
        /// <param name="name">Product's name</param>
        /// <param name="description">Product's description</param>
        /// <param name="rarity">Product's rarity</param>
        /// <param name="game">Product's game appartenence</param>
        /// <param name="expansion">Product's expansion appartenence</param>
        void AddProduct(string name, string description, string rarity, string game, string expansion);

        /// <summary>
        /// Edit product information.
        /// </summary>
        /// <param name="product">New product information</param>
        void EditProduct(Product product);

        /// <summary>
        /// Add a new expansion.
        /// </summary>
        /// <param name="name">The new expansion name</param>
        void AddExpansion(string name);

        /// <summary>
        /// Add a new game.
        /// </summary>
        /// <param name="name">The new game name</param>
        void AddGame(string name);

        /// <summary>
        /// Add a new rarity.
        /// </summary>
        /// <param name="name">The new rarity name</param>
        void AddRarity(string name);

        /// <summary>
        /// Add a new condition.
        /// </summary>
        /// <param name="name">The new condition name</param>
        void AddCondition(string name);

        /// <summary>
        /// Completing a buy operation
        /// </summary>
        /// <param name="userUsername">Buyer's username</param>
        /// <param name="sellerUsername">Seller's username</param>
        /// <param name="feedback">Buyer's feedback</param>
        /// <param name="details">Details of buyer's purchases</param>
        /// <param name="coupons">Codici coupon utilizzati</param>
        void Buy(string userUsername, string sellerUsername, Feedback feedback, List<Detail> details, List<string> coupons);

        /// <summary>
        /// Add a product to an user wishlist.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="productID">Product ID</param>
        /// <param name="quantity">Quantity of desired product</param>
        void AddToWishlist(string username, int productID, int quantity);

        /// <summary>
        /// Get the list of products in the user's wishlist
        /// </summary>
        /// <param name="username">The target user</param>
        /// <returns>A list of products in the user's wishlist</returns>
        List<WishlistItem> GetWishlist(string username);

        /// <summary>
        /// Find every offert for the given product
        /// </summary>
        /// <param name="productId">The product to search the offerts for</param>
        /// <returns>The list of offerts for that product</returns>
        List<Offert> GetOfferts(int productId);

        /// <summary>
        /// Get user's account informations
        /// </summary>
        /// <param name="username">User's username</param>
        /// <returns>Informations about user's account</returns>
        Account GetUserAccount(string username);

        /// <summary>
        /// Get bank account's informations
        /// </summary>
        /// <param name="bankAccountId"> Id of the bank account </param>
        /// <returns> A BankAccount object containing informations related informations </returns>
        BankAccount GetBankAccount(int bankAccountId);

        /// <summary>
        /// Create a coupon.
        /// </summary>
        /// <param name="username">User who create the coupon</param>
        /// <param name="value">Value of the value of the coupon</param>
        void CreateCoupon(string username, int value);

        /// <summary>
        /// Link a bank account to an user account.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="bankAccount">Bank account informations</param>
        void AddUserBankAccount(string username, BankAccount bankAccount);

        /// <summary>
        /// Update informations about bankAccounts.
        /// </summary>
        /// <param name="bankAccount">Bank account's informations</param>
        void UpdateUserBankAccount(BankAccount bankAccount);

        /// <summary>
        /// Remove the bank account linked to the user account.
        /// </summary>
        /// <param name="account">User's account</param>
        void RemoveUserBankAccount(Account account);
    }
}
