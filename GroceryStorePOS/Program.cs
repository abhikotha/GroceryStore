using GroceryStorePOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GroceryStorePOS
{
    class Program
    {
        /// <summary>
        /// cart for adding products
        /// </summary>
        static List<CartModel> Cart = new List<CartModel>();
        
        /// <summary>
        /// 
        /// </summary>
        static ProductPriceCatalog productPriceCatalog = new ProductPriceCatalog();
        
        /// <summary>
        /// flag for stop adding product in cart
        /// </summary>
        static bool flag = true;
        static void Main(string[] args)
        {

            AddProduct(); // Add product to cart

            CaculateCartPrice(); // caclulate product price

            PrintReceipt(); // print receipt

            Console.ReadLine();
        }
        
        /// <summary>
        /// calculate and update cart with amount
        /// </summary>
        static void CaculateCartPrice()
        {
            var productPriceCatalog = new ProductPriceCatalog();
            var modifiedCart = new List<CartModel>();
            foreach (var item in Cart)
            {

                var remainingAdditionalQty = 0;

                var remainingGrouplQty = 0;

                decimal additionalProductDiscountAmount = productPriceCatalog.GetAdditionalProductDiscounts(item, ref remainingAdditionalQty);

                decimal groupPromotionalPriceAmount = productPriceCatalog.CalulateGroupPromotionalPriceDiscount(item, ref remainingGrouplQty);

                decimal appliedDiscount = 0;

                if (groupPromotionalPriceAmount >= additionalProductDiscountAmount && remainingGrouplQty < remainingAdditionalQty)
                {
                    appliedDiscount = groupPromotionalPriceAmount;
                    item.Qty -= remainingGrouplQty;
                    if (remainingGrouplQty > 0)
                    {


                        var cartModel = new CartModel()
                        {
                            Qty = remainingGrouplQty,
                            ProductId = item.ProductId,
                            Product = item.Product

                        };
                        cartModel = productPriceCatalog.GetPriceByProduct(cartModel);
                        cartModel.Discount = (cartModel.Price * cartModel.Qty * cartModel.Discount) / 100;
                        cartModel.Amount = (cartModel.Price * cartModel.Qty) - cartModel.Discount;
                        modifiedCart.Add(cartModel);
                    }
                }
                else if (additionalProductDiscountAmount > 0)
                {
                    appliedDiscount = additionalProductDiscountAmount;
                    item.Qty -= remainingAdditionalQty;
                    if (remainingAdditionalQty > 0)
                    {
                        var cartModel = new CartModel()
                        {
                            Qty = remainingAdditionalQty,
                            ProductId = item.ProductId,
                            Product = item.Product

                        };
                        cartModel = productPriceCatalog.GetPriceByProduct(cartModel);
                        cartModel.Discount = (cartModel.Price * cartModel.Qty * cartModel.Discount) / 100;
                        cartModel.Amount = (cartModel.Price * cartModel.Qty) - cartModel.Discount;
                        modifiedCart.Add(cartModel);
                    }
                }

                if (item.Discount > appliedDiscount)
                {
                    appliedDiscount = item.Discount;

                }

                item.Discount = appliedDiscount;
                item.Amount = (item.Price * item.Qty) - item.Discount;
                modifiedCart.Add(item);
            }
            Cart = modifiedCart;
        }

        /// <summary>
        /// print cart products 
        /// </summary>
        static void PrintReceipt()
        {

            Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,20}|{3,20}|{4,20}|", "Qty", "Description", "Price", "Discount", "Amount"));
            foreach (var product in Cart)
            {
                Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,20}|{3,20}|{4,20}|", product.Qty, product.Product, "(" + product.Qty + " x $" + product.Price.ToString("0.00") + " ) " + (product.Qty * product.Price).ToString("0.00"), "$" + product.Discount.ToString("0.00"), "$" + product.Amount.ToString("0.00")));

                Console.WriteLine();
            }
            Console.WriteLine(String.Format(" {0,20} {1,20} {2,20}|{3,20}|{4,20}|", "", "", "Gross Price: $" + Cart.Sum(x => x.Price * x.Qty).ToString("0.00"), "Total Discount: $" + Cart.Sum(x => x.Discount).ToString("0.00"), "Net Amount: $" + Cart.Sum(x => x.Amount).ToString("0.00")));
        }

        /// <summary>
        /// recursive function for adding product in to cart
        /// 
        /// </summary>
        static void AddProduct()
        {
            if (flag)
            {
                Console.WriteLine("Enter Product Name : ");
                var product = Console.ReadLine();
                if (product == string.Empty)
                {
                    flag = false;
                    return;
                }
                Console.WriteLine("Enter Product Qty : ");

                int qty;

                if (!int.TryParse(Console.ReadLine(), out qty))
                {
                    Console.WriteLine("Invalid Qty");
                    AddProduct();
                }

                var cartModel = new CartModel()
                {
                    Product = product,
                    Qty = qty

                };

                cartModel = productPriceCatalog.GetPriceByProduct(cartModel);

                var cartProduct = Cart.Where(x => x.ProductId == cartModel.ProductId).FirstOrDefault();

                if (cartProduct == null)
                {
                    cartModel.Discount = (cartModel.Price * cartModel.Qty * cartModel.Discount) / 100;
                    Cart.Add(cartModel);

                }
                else
                {
                    cartProduct.Qty += cartModel.Qty;
                    cartProduct.Discount = (cartModel.Price * cartModel.Qty * cartModel.Discount) / 100;
                }

                AddProduct();
            }
        }


    }
}
