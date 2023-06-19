﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Product(int productId, string name, DateTime date, string description, 
        string rarity, string game, string expansion);
}
