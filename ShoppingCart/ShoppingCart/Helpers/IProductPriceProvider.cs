using System.Net;
using ShoppingCart.Model;

namespace ShoppingCart.Helpers;

public interface IProductPriceProvider
{
    Task<decimal> GetPrice(string productName);
}