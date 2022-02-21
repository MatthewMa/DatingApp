using API.Data;
using API.DTOS;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Tools;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{   
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]        
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetAllAppUsers([FromQuery] UserParams userParams)
        {
            var username = User.GetUserName();
            var user = await _userRepository.GetUserByUserNameAsync(username);
            if (user == null)
            {
                return BadRequest(Constants.USER_NOT_FOUND);
            }
            userParams.CurrentUserName = username;
            userParams.Gender ??= (user.Gender.ToUpper() == "Male".ToUpper()) ? "Female" : "Male";
            var members = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(userParams.PageNumber, userParams.PageSize, members.TotalCount, members.TotalPages);
            return Ok(members.List);
        }
       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDTO>> GetAppUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return BadRequest(Constants.USER_NOT_FOUND);
            var member = _mapper.Map<MemberDTO>(user);
            return Ok(member);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetAppUserByName(string username)
        {
            var member = await _userRepository.GetMemberByUserNameAsync(username);
            if (member == null)
                return BadRequest(Constants.USER_NOT_FOUND);           
            return Ok(member);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAppUser(MemberUpdateDTO memberUpdateDTO)
        {
            // Search the user            
            AppUser user = await GetUserByUserName();
            if (user == null) return BadRequest("User name does not exist");
            _mapper.Map(memberUpdateDTO, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("User update failed");
        }

        private async Task<AppUser> GetUserByUserName()
        {
            var username = User.GetUserName();
            if (string.IsNullOrEmpty(username))
                return null;
            return await _userRepository.GetUserByUserNameAsync(username);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhotoAsync(IFormFile file)
        {
            var user = await GetUserByUserName();
            if (user == null) return BadRequest("User name does not exist");
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync()) 
            {
                return CreatedAtRoute("GetUser", new { username=user.UserName}, _mapper.Map<PhotoDTO>(photo));
            }
            return BadRequest("Photo saved failed.");
        }

        [HttpPut("set-main-photo")]
        public async Task<ActionResult> SetMainPhoto([FromBody] PhotoUpdateDTO photoUpdateDTO)
        {
            var user = await GetUserByUserName();
            if (user == null) return BadRequest("User name does not exist");         
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoUpdateDTO.id);
            if (photo == null) return BadRequest("Photo doe not exist");
            // Set others to false
            foreach (var item in user.Photos)
            {
                item.IsMain = false;
            }
            photo.IsMain = true;
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync())
                return NoContent();
            return BadRequest("Something went wrong");
        }

        [HttpDelete("delete-photo/{id:int}")]
        public async Task<ActionResult> DeletePhoto(int id)
        {
            var user = await GetUserByUserName();
            if (user == null) return BadRequest("User name does not exist");
            var photo = user.Photos.FirstOrDefault(p => p.Id == id);
            if (photo == null) return BadRequest("Photo doe not exist");
            user.Photos.Remove(photo);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync())
                return NoContent();
            return BadRequest("Something went wrong");
        }
    }
}
