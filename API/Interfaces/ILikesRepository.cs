using API.DTOS;
using API.Entities;
using API.Tools;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikesAsync(int userId);
        Task<PagedList<LikeDTO>> GetUserLikesAsync(LikeParams likeParams);        
        Task<bool> SaveAllAsync();
    }
}
