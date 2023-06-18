using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Product(int id, string name, DateTime date, 
        string description, string rarita, string gioco, string espansione);
}
