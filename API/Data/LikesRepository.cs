using API.DTOS;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Tools;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LikesRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }      

        public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikesAsync(LikeParams likeParams)
        {
            var users = _context.AppUsers.AsQueryable();
            var likes = _context.Likes.AsQueryable();           
            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser);
            } 
            else
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }
            users = likeParams.OrderBy switch
            {
                "created" => users.OrderByDescending(u => u.Created),
                "age" => users.OrderByDescending(u => u.DateOfBirth),
                _ => users.OrderByDescending(u => u.LastActive),
            };
            return await PagedList<LikeDTO>.CreateAsync(users.ProjectTo<LikeDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking(), likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikesAsync(int userId)
        {
            return await _context.AppUsers.Include(u => u.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);               
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
