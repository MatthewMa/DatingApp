using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using API.Tools;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{   
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]        
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetAllAppUsers()
        {
            var members = await _userRepository.GetMembersAsync();           
            return Ok(members);
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

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetAppUserByName(string username)
        {
            var member = await _userRepository.GetMemberByUserNameAsync(username);
            if (member == null)
                return BadRequest(Constants.USER_NOT_FOUND);           
            return Ok(member);
        }

        [HttpPut]
        public async Task<ActionResult<MemberDTO>> UpdateAppUser(AppUser user)
        {
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync())
            {
                var member = _mapper.Map<MemberDTO>(user);
                return Ok(member);
            }                
            return BadRequest("User update failed");
        }
    }
}
