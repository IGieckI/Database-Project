using Database_Project.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public static class Session
    {
        public static string Username = "";

        public static LoginType Login = LoginType.None;
        public enum LoginType
        {
            None,
            User,
            Seller,
            Admin
        }

        public static List<Detail> shoppingCart = new List<Detail>();

        public static IDatabase Database = new DatabaseImpl();
    }
}
