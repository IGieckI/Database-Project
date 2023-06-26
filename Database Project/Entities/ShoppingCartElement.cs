using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record ShoppingCartElement(string Name, decimal Price, int Quantity, string Seller);
}
