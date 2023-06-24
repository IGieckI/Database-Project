using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Account(string Username, string Email, string Password, string? Street, int? CivicNumber, int? Cap, 
        string? City, string? Country, string TelephoneNumber, int Credit, int? BankAccountID);
}
