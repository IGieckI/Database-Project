using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record BankAccount(int BankAccountID, string IBAN, string BankName, string BIC_SWIFT);
}
