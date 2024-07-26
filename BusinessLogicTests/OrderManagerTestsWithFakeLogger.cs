using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;

namespace BusinessLogicTests
{
    [TestFixture]
    public class OrderManagerTestsWithFakeLogger
    {
        private Mock<IExternalCustomerApiClient> _customerApiClientMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private FakeLogger<OrderManager> _fakeLogger;
        private IOrderManager _orderManager;

        [SetUp]
        public void Setup()
        {
            _customerApiClientMock = new Mock<IExternalCustomerApiClient>(MockBehavior.Default);
            _orderRepositoryMock = new Mock<IOrderRepository>(MockBehavior.Default);
            _fakeLogger = new FakeLogger<OrderManager>();
            _orderManager = new OrderManager(_orderRepositoryMock.Object, _customerApiClientMock.Object, _fakeLogger);
        }

        [Test]
        public async Task OrderManager_Should_Logwarning_If_Customer_NotFound()
        {
            _customerApiClientMock.Setup(x => x.GetCustomerAsync(It.IsAny<string>())).ReturnsAsync((Customer?)null);
            var orderId = await _orderManager.CreateOrderAsync("test@gmail.com", "Iphone", 2);
            _fakeLogger.Collector.LatestRecord.Should().NotBeNull();
            _fakeLogger.Collector.LatestRecord.Message.Should().Be("No customer found for email provided.");
            _fakeLogger.Collector.LatestRecord.Level.Should().Be(LogLevel.Warning);
            _fakeLogger.Collector.Count.Should().Be(2);
        }

        [Test]
        public async Task OrderManager_Should_Logerror_In_Case_Exception()
        {
            var options = new FakeLogCollectorOptions()
            {
                //We can override diabled log levels and collect them
                CollectRecordsForDisabledLogLevels = true,
                //Write the log messages to console
                OutputSink = Console.WriteLine
            };
            //Filter to certain levels for validation
            options.FilteredLevels.Add(LogLevel.Error);
            options.FilteredLevels.Add(LogLevel.Warning);
            var collection = FakeLogCollector.Create(options);
            var fakeLogger = new FakeLogger<OrderManager>(collection);

            _orderManager = new OrderManager(_orderRepositoryMock.Object, _customerApiClientMock.Object, fakeLogger);

            _customerApiClientMock.Setup(x => x.GetCustomerAsync(It.IsAny<string>())).ReturnsAsync(new Customer() { 
                CustomerId = 1212,
                Email = "test@gmail.com",
                FirstName = "FN",
                LastName = "LN"
            });
            _orderRepositoryMock.Setup(x => x.CreateOrderAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((Guid?)null);
            var orderId = await _orderManager.CreateOrderAsync("test@gmail.com", "Iphone", 2);
            fakeLogger.Collector.LatestRecord.Should().NotBeNull();
            fakeLogger.Collector.LatestRecord.Message.Should().Be("Order not created, please check logs for more details.");
            fakeLogger.Collector.LatestRecord.Level.Should().Be(LogLevel.Warning);
            //Since we filtered to just warning you will only get 1 record
            fakeLogger.Collector.Count.Should().Be(1);
        }
    }
}
