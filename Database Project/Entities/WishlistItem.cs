using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record WishlistItem(string Name, int Quantity, string Rarity, string Game, string Expansion);
}
