using BusinessLogic;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessLogicTests
{
    [TestFixture]
    public class OrderManagerTestsWithoutFakeLogger
    {
        private Mock<IExternalCustomerApiClient> _customerApiClientMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<ILogger<OrderManager>> _loggerMock;
        private IOrderManager _orderManager;

        [SetUp]
        public void Setup()
        {
            _customerApiClientMock = new Mock<IExternalCustomerApiClient>(MockBehavior.Default);
            _orderRepositoryMock = new Mock<IOrderRepository>(MockBehavior.Default);
            _loggerMock = new Mock<ILogger<OrderManager>>(MockBehavior.Default);  
            _orderManager = new OrderManager(_orderRepositoryMock.Object, _customerApiClientMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task OrderManager_Should_Logwarning_If_Customer_NotFound()
        {
            _customerApiClientMock.Setup(x => x.GetCustomerAsync(It.IsAny<string>())).ReturnsAsync((Customer?)null);
            var orderId = await _orderManager.CreateOrderAsync("test@gmail.com", "Iphone", 2);
            orderId.Should().BeNull();
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("No customer found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once());
        }
    }
}
