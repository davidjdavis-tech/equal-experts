using System.Net;
using RichardSzalay.MockHttp;
using ShoppingCart.Helpers;

namespace ShoppingCart.Tests;

[TestFixture]
public class ProductPriceClientTests
{
    private MockHttpMessageHandler _mockHttp;
    private ProductPriceClient _productPriceClient;
    private const string Url = "https://equalexperts.github.io/";

    [SetUp]
    public void SetUp()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new System.Uri(Url);

        _productPriceClient = new ProductPriceClient(httpClient);
    }

    [Test]
    public async Task GetPrice_ValidProduct_ReturnsPrice()
    {
        // Arrange
        var productName = "TestProduct";
        var expectedPrice = 19.99m;
        _mockHttp.When($"{Url}{productName}")
            .Respond("application/json", expectedPrice.ToString()); // Respond with JSON content

        // Act
        var price = await _productPriceClient.GetPrice(productName);

        // Assert
        Assert.That(price, Is.EqualTo(expectedPrice));
    }

    [Test]
    public void GetPrice_InvalidProduct_ThrowsException()
    {
        // Arrange
        var productName = "InvalidProduct";
        _mockHttp.When($"{Url}{productName}")
            .Respond(HttpStatusCode.NotFound); // Simulate a 404 Not Found response

        // Act & Assert
        var ex = Assert.ThrowsAsync<System.Exception>(async () => await _productPriceClient.GetPrice(productName));
        Assert.IsTrue(ex.Message.Contains($"Failed to get product info for {productName}"));
    }
}