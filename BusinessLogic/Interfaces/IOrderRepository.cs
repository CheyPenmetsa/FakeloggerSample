namespace BusinessLogic.Interfaces
{
    public interface IOrderRepository
    {
        Task<Guid?> CreateOrderAsync(int customerId, string productName, int productQuantity);
    }
}
