using Moq;
using ShoppingCart.Helpers;

namespace ShoppingCart.Tests
{
    [TestFixture]
    public class CartTests
    {
        private Mock<IProductPriceProvider> _mockProductInfoProvider;
        private Cart _cart;

        [SetUp]
        public void SetUp()
        {
            // Mock the IProductInfoProvider interface
            _mockProductInfoProvider = new Mock<IProductPriceProvider>();

            // Initialize the Cart with the mocked product info provider
            _cart = new Cart(_mockProductInfoProvider.Object);
        }

        [Test]
        public void TwoCornflakesOneWeetabix()
        {
            // Arrange
            _mockProductInfoProvider.Setup(m => m.GetPrice("weetabix")).ReturnsAsync(9.98m);
            _mockProductInfoProvider.Setup(m => m.GetPrice("cornflakes")).ReturnsAsync(2.52m);

            // Act
            _cart.AddProduct("cornflakes", 1);
            _cart.AddProduct("cornflakes", 1);
            _cart.AddProduct("weetabix", 1);
            var state = _cart.GetCartState();

            // Assert
            Assert.That(_cart.GetItemNames(), Does.Contain("cornflakes"));
            Assert.That(_cart.GetItemNames(), Does.Contain("weetabix"));
            
            Assert.That(state.Subtotal, Is.EqualTo(15.02d));
            Assert.That(state.TaxPayable, Is.EqualTo(1.88d));
            Assert.That(state.TotalPayable, Is.EqualTo(16.90d));
        }
        
        [Test]
        public void AddProduct_NewProduct_AddsProductSuccessfully()
        {
            // Arrange
            var productName = "TestProduct";
            var quantity = 5;
            var productPrice = 10.0m;
            _mockProductInfoProvider.Setup(m => m.GetPrice(productName)).ReturnsAsync(productPrice);

            // Act
            _cart.AddProduct(productName, quantity);
            var state = _cart.GetCartState();

            // Assert
            Assert.That(productName, Is.EqualTo(_cart.GetItemNames().FirstOrDefault()));
            Assert.That(productPrice * quantity, Is.EqualTo(state.Subtotal));
            
        }

        [Test]
        public void AddProduct_ExistingProduct_UpdatesQuantitySuccessfully()
        {
            // Arrange
            string productName = "TestProduct";
            int initialQuantity = 3;
            int additionalQuantity = 2;
            decimal productPrice = 10.0m;
            _mockProductInfoProvider.Setup(m => m.GetPrice(productName)).ReturnsAsync(productPrice);
            _cart.AddProduct(productName, initialQuantity);

            // Act
            _cart.AddProduct(productName, additionalQuantity);

            // Assert
            var state = _cart.GetCartState();
            Assert.That(productPrice * (initialQuantity + additionalQuantity), Is.EqualTo(state.Subtotal));
        }

        [Test]
        public void GetCartState_EmptyCart_ReturnsZeroValues()
        {
            // Act
            var state = _cart.GetCartState();

            // Assert
            Assert.That(0, Is.EqualTo(state.Subtotal));
            Assert.That(0, Is.EqualTo(state.TaxPayable));
            Assert.That(0, Is.EqualTo(state.TotalPayable));
            Assert.That(0, Is.EqualTo(_cart.GetItemNames().Count));
        }

        [Test]
        public void GetCartState_NonEmptyCart_CalculatesTotalsCorrectly()
        {
            // Arrange
            string productName = "TestProduct";
            int quantity = 2;
            decimal productPrice = 20.0m;
            double expectedTaxRate = 12.5;
            _mockProductInfoProvider.Setup(m => m.GetPrice(productName)).ReturnsAsync(productPrice);
            _cart.AddProduct(productName, quantity);

            // Act
            var state = _cart.GetCartState();

            // Assert
            var expectedSubtotal = Math.Round(productPrice * quantity, 2);
            var expectedTax = Math.Round(expectedSubtotal * (decimal)(expectedTaxRate / 100), 2);
            var expectedTotal = expectedSubtotal + expectedTax;

            Assert.That(expectedSubtotal, Is.EqualTo(state.Subtotal));
            Assert.That(expectedTax, Is.EqualTo(state.TaxPayable));
            Assert.That(expectedTotal, Is.EqualTo(state.TotalPayable));
        }
    }
}