using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Coupon(string CouponCode, DateTime ExpireDate, decimal Value, string UserGenerator, string UserUtilizer);
}
