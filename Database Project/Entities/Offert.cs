﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Offert(int OffertId, decimal Price, int Quantity, string Conditions, string Language, string Location, int ProductId, string SellerUsername);
}
