using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroceryStorePOS.Models;
using System.IO;

namespace GroceryStorePOS
{
    public class ProductPriceCatalog
    {

        public CartModel GetPriceByProduct(CartModel cartModel)
        {
            var product = GetProductByName(cartModel.Product);
            if (product != null)
            {
                cartModel.Price = product.Price;
                cartModel.ProductId = product.ProductId;
                cartModel.Discount = product.Discount;

            }
            return cartModel;
        }
        private ProductModel GetProductByName(string product)
        {

            StreamReader reader = new StreamReader(File.OpenRead(Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "") + "/App_Data/Product.csv"));

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values[1] == product)
                {
                    var productModel = new ProductModel();
                    productModel.ProductId = Convert.ToInt32(values[0]);
                    productModel.Product = product;
                    productModel.Price = Convert.ToDecimal(values[2]); ;
                    productModel.Discount = Convert.ToDecimal(values[3]); ;
                    return productModel;
                }
            }

            return null;

        }
        public List<string> GetAllProducts()
        {
            var products = new List<string>();
            StreamReader reader = new StreamReader(File.OpenRead(Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "") + "/App_Data/Product.csv"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values[0].ToLower() != "id")
                {
                    products.Add(values[1]);
                }
            }
            return products;
        }
        private List<AdditionalProductDiscountModel> GetAllAdditionalProductDiscount()
        {
            var lstAdditionalProductDiscounts = new List<AdditionalProductDiscountModel>();
            StreamReader reader = new StreamReader(File.OpenRead(Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "") + "/App_Data/AdditionalProductDiscount.csv"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values[1].ToLower() != "productid" && Convert.ToBoolean(values[4]))
                {
                    lstAdditionalProductDiscounts.Add(new AdditionalProductDiscountModel()
                    {
                        Discount = Convert.ToDecimal(values[3]),
                        Qty = Convert.ToInt32(values[2]),
                        ProductId = Convert.ToInt32(values[1])
                    });
                }
            }
            return lstAdditionalProductDiscounts;
        }
        private List<GroupPromotionalPriceModel> GetAllGroupPromotionalPrice()
        {
            var lstGroupPromotionalPrice = new List<GroupPromotionalPriceModel>();
            StreamReader reader = new StreamReader(File.OpenRead(Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "") + "/App_Data/GroupPromotionalPrice.csv"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values[1].ToLower() != "productid" && Convert.ToBoolean(values[4]))
                {
                    lstGroupPromotionalPrice.Add(new GroupPromotionalPriceModel()
                    {
                        Price = Convert.ToDecimal(values[3]),
                        Qty = Convert.ToInt32(values[2]),
                        ProductId = Convert.ToInt32(values[1])

                    });
                }
            }
            return lstGroupPromotionalPrice;
        }

        public decimal GetAdditionalProductDiscounts(CartModel cartModel, ref int remainingQty)
        {

            var lstAdditionalProductDiscounts = GetAllAdditionalProductDiscount();

            var additionalProductDiscount = lstAdditionalProductDiscounts.Where(x => x.Qty <= cartModel.Qty && x.ProductId == cartModel.ProductId).OrderByDescending(c => c.Qty).FirstOrDefault();

            if (additionalProductDiscount != null)
            {
                return CalculateAdditionalProductDiscounts(additionalProductDiscount, cartModel.Qty - 1, cartModel.Price, 0, ref remainingQty);
            }
            return 0;

        }
        private decimal CalculateAdditionalProductDiscounts(AdditionalProductDiscountModel additionalProductDiscountModel, int qty, decimal price, decimal discount, ref int remainingQty)
        {
            if (additionalProductDiscountModel.Qty <= qty)
            {
                //var grossAmount = (additionalProductDiscountModel.Qty + 1) * price;

                discount +=  (additionalProductDiscountModel.Qty * price * additionalProductDiscountModel.Discount) / 100;

                qty -= additionalProductDiscountModel.Qty + 1;

                return CalculateAdditionalProductDiscounts(additionalProductDiscountModel, qty, price, discount, ref remainingQty);
            }
            else
                remainingQty = qty + 1;


            return discount;

        }
        private decimal CalculateGroupPromotionalPrice(GroupPromotionalPriceModel groupPromotionalPriceModel, int qty, decimal price, decimal discount, ref int remainingQty)
        {
            if (groupPromotionalPriceModel.Qty <= qty)
            {
                var grossAmount = groupPromotionalPriceModel.Qty * price;

                discount += (grossAmount - groupPromotionalPriceModel.Price);

                qty -= groupPromotionalPriceModel.Qty;

                return CalculateGroupPromotionalPrice(groupPromotionalPriceModel, qty, price, discount, ref remainingQty);
            }
            else
                remainingQty = qty;

            return discount;

        }
        public decimal CalulateGroupPromotionalPriceDiscount(CartModel cartModel, ref int remainingQty)
        {
            var lstGroupPromotionalPrices = GetAllGroupPromotionalPrice();

            var groupPromotionalPrice = lstGroupPromotionalPrices.Where(x => x.Qty <= cartModel.Qty && x.ProductId == cartModel.ProductId).OrderByDescending(c => c.Qty).FirstOrDefault();

            if (groupPromotionalPrice != null)
            {
                return CalculateGroupPromotionalPrice(groupPromotionalPrice, cartModel.Qty, cartModel.Price, 0, ref remainingQty);
            }

            return 0;
        }
    }
}
