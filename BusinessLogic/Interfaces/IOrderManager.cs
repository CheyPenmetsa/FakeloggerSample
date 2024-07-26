namespace BusinessLogic.Interfaces
{
    public interface IOrderManager
    {
        Task<Guid?> CreateOrderAsync(string email, string productName, int productQuantity);
    }
}
