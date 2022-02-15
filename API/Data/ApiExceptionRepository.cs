using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApiExceptionRepository : IApiExceptionRepository
    {
        private readonly DataContext _context;

        public ApiExceptionRepository(DataContext context)
        {
            _context = context;
        }

        public void AddException(ApiException exception)
        {
            _context.ApiExceptions.Add(exception);
        }

        public async Task<ApiException> GetApiExceptionByIdAsync(int id)
        {
            return await _context.ApiExceptions.FindAsync(id);
        }

        public async Task<IEnumerable<ApiException>> GetExceptionsAsync()
        {
            return await _context.ApiExceptions.ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
