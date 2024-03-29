using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using ShoppingCart.Model;

namespace ShoppingCart.Helpers;

public class ProductPriceClient : IProductPriceProvider
{
    private readonly HttpClient _httpClient;
    private const string Url = "https://equalexperts.github.io/";

    public ProductPriceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<decimal> GetPrice(string productName)
    {
        try
        {
            var response =  await _httpClient.GetFromJsonAsync<decimal>($"{Url}{productName}");
            return response;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get product info for {productName}. {e.Message}");
        }
    }
}