using API.DTOS;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikesAsync(int userId);
        Task<IEnumerable<LikeDTO>> GetUserLikesAsync(string predicate, int userId);        
        Task<bool> SaveAllAsync();
    }
}
