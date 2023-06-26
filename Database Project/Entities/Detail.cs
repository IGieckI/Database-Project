using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Detail(int DetailId, decimal Price, int Quantity, int OffertId, int SellId);
}
