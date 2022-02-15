using API.Entities;

namespace API.Interfaces
{
    public interface IApiExceptionRepository
    {
        void AddException(ApiException exception);
        Task<IEnumerable<ApiException>> GetExceptionsAsync();
        Task<ApiException> GetApiExceptionByIdAsync(int id);
        Task<bool> SaveAllAsync();
    }
}
