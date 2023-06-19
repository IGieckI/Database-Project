using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record User(string username, string email, string way, int civicAddress, int cap, 
        string city, string country, int telephoneNumber);
}
