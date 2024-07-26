using BusinessLogic.Models;

namespace BusinessLogic.Interfaces
{
    public interface IExternalCustomerApiClient
    {
        Task<Customer?> GetCustomerAsync(string email);
    }
}
