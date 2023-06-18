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
    internal interface Database
    {
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
        /// Retrive from database all the information about the product asked for.
        /// </summary>
        /// <param name="productName"> The entire or a part of the product name</param>
        /// <returns>A List of Product contained in the database</returns>
        List<Product> GetProducts(string productName = "");
    }
}
