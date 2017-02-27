using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStorePOS.Models
{
    public class CartModel
    {
        public int ProductId { get; set; }
        public string Product { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
