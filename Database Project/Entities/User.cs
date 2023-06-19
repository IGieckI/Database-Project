using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record User(string Username, string Email, string Password, string Street, int CivicAddress, int Cap, 
        string City, string Country, int TelephoneNumber);
}
