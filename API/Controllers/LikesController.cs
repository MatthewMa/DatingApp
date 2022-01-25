using API.DTOS;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            // Get logged user id
            int sourceUserId = User.GetUserId();
            var sourcedUser = await _likesRepository.GetUserWithLikesAsync(sourceUserId);
            var user = await _userRepository.GetUserByUserNameAsync(username);
            if (user == null) return BadRequest("User does not exist");
            int likedUserId = user.Id;
            if (likedUserId == sourceUserId) return BadRequest("You cannot like yourself");
            var userLike = await _likesRepository.GetUserLikeAsync(sourceUserId, likedUserId);
            if (userLike != null) return BadRequest("You already liked this user");
            sourcedUser.LikedUsers.Add(new Entities.UserLike
            {
                LikedUserId = likedUserId,
                SourceUserId = sourceUserId
            });
            if (!await _userRepository.SaveAllAsync()) return BadRequest("Like user saved failed");
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetLikedUsers([FromQuery] string predicate = "liked")
        {
            int userId = User.GetUserId();
            if (userId == 0) return BadRequest("User does not exist");            
            var result = await _likesRepository.GetUserLikesAsync(predicate, userId);
            return Ok(result);
        }
    }   
}
