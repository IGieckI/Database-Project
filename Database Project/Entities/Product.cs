using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Product(int ProductId, string Name, DateTime Date, string Description, 
        string Rarity, string Game, string Expansion);
}
