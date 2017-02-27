using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStorePOS.Models
{
    class AdditionalProductDiscountModel
    {
        public int ID { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
    }
}
