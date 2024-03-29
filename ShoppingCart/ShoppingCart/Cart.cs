using System.Text;
using ShoppingCart.Helpers;
using ShoppingCart.Model;

namespace ShoppingCart;

public class Cart
{
    private readonly IProductPriceProvider _productPriceProvider;
    private readonly List<Product> _items;
    private const double TaxRate = 12.5;

    public Cart(IProductPriceProvider productPriceProvider)
    {
        _productPriceProvider = productPriceProvider;
        _items = new List<Product>();
    }
    
    public void AddProduct(string name, int quantity)
    {
        var existingItem = _items.FirstOrDefault(x => x.Name.Equals(name));
        
        // just update quantity if item is already in the list
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new Product { Price = _productPriceProvider.GetPrice(name).Result, Quantity = quantity, Name = name};
            _items.Add(cartItem);
        }
    }

    public State GetCartState()
    {
        if (_items.Count == 0)
        {
            return new State { Subtotal = 0, TaxPayable = 0, TotalPayable = 0 };
        }

        var subtotal = Math.Round(_items.Sum(item => item.Price * item.Quantity), 2);
        var tax = Math.Round(subtotal * (decimal)(TaxRate/100), 2); 
        var total = subtotal + tax;
        return new State { Subtotal = subtotal, TaxPayable = tax, TotalPayable = total };
    }

    public List<string> GetItemNames()
    {
        return _items.Select(x => x.Name).ToList();
    }
}
