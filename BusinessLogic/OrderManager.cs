using BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;

namespace BusinessLogic
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderRepository _orderRepository;
               
        private readonly IExternalCustomerApiClient _externalCustomerApiClient;

        private readonly ILogger<OrderManager> _logger;

        public OrderManager(IOrderRepository orderRepository, 
            IExternalCustomerApiClient externalCustomerApiClient, 
            ILogger<OrderManager> logger)
        {
            _orderRepository = orderRepository;
            _externalCustomerApiClient = externalCustomerApiClient;
            _logger = logger;
        }

        public async Task<Guid?> CreateOrderAsync(string email, string productName, int productQuantity)
        {
            try
            {
                _logger.LogDebug($"CreateOrderAsync method called using customer email: {email} for product: {productName} with quantity: {productQuantity}");

                var customer = await _externalCustomerApiClient.GetCustomerAsync(email);

                if (customer == null)
                {
                    _logger.LogWarning("No customer found for email provided.");
                    return null;
                }

                var orderId = await _orderRepository.CreateOrderAsync(customer.CustomerId, productName, productQuantity);
                if(orderId == null)
                {
                    _logger.LogWarning("Order not created, please check logs for more details.");
                }

                return orderId;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"CreateOrderAsync got error: {ex.ToString()}");
                throw;
            }
        }
    }
}
