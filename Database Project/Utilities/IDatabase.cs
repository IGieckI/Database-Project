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
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <param name="email">User's email</param>
        /// <returns>True if registered correctly, false otherwise</returns>
        bool UserRegistration(string username, string password, string email);

        /// <summary>
        /// Register a new Seller.
        /// </summary>
        /// <param name="username">Seller's username</param>
        /// <param name="password">Seller's password</param>
        /// <param name="email">Seller's email</param>
        /// <param name="country">Seller's country</param>
        /// <param name="IBAN">Seller's bank account's IBAN</param>
        /// <param name="bankName">Seller's bank's name</param>
        /// <param name="bicSwiftCode">Seller's bank account's BIC/SWIFT code</param>
        /// <returns>True if registered correctly, false otherwise</returns>
        bool SellerRegistration(string username, string password, string email, string country, string IBAN, string bankName, string bicSwiftCode);

        /// <summary>
        /// Register a new Admin.
        /// </summary>
        /// <param name="username">Admin's username</param>
        /// <param name="password">Admin's password</param>
        /// <param name="email">Admin's email</param>
        /// <returns>True if registered correctly, false otherwise</returns>
        bool AdminRegistration(string username, string password, string email);

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
        void EditUserProfile(User user);

        /// <summary>
        /// Retrive from database all the information about the product asked for.
        /// </summary>
        /// <param name="productName"> The entire or a part of the product name</param>
        /// <returns>A List of Product contained in the database</returns>
        List<Product> GetProducts(string productName = "");

        /// <summary>
        /// A Seller adding an offert.
        /// </summary>
        /// <param name="username">Seller's username</param>
        /// <param name="price">Single product's price</param>
        /// <param name="quantity">Quantity of product</param>
        /// <param name="language">Language of product</param>
        /// <param name="location">Product location</param>
        /// <param name="conditions">Product conditions</param>
        void AddOffert(string username, float price, int quantity, string language, string location, string conditions);

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
        /// Create a coupon.
        /// </summary>
        /// <param name="username">User who create the coupon</param>
        /// <param name="value">Value of the value of the coupon</param>
        void CreateCoupon(string username, int value);

        /// <summary>
        /// Link a bank account to an user account.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="IBAN">Bank account's IBAN</param>
        /// <param name="bankName">Bank's name</param>
        /// <param name="BicSwift">BIC/SWIFT code</param>
        void AddUserBankAccount(string username, string IBAN, string bankName, string BicSwift);
    }
}
