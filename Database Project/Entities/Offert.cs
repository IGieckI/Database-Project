using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Offert(int Id, float price, int quantity, string conditions, string language, string location, int ProductId);
}
