using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using API.Tools;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager ,
            ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AccountReturnDTO>> Register([FromBody] AccountRegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
            {
                return BadRequest(Constants.REGISTER_USERNAME_EXIST);
            }            
            var user = _mapper.Map<AppUser>(registerDTO);
            user.UserName = registerDTO.UserName.ToLower();                       
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            // Add user to member role
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            // Set AccountLoginReturnDTO
            AccountReturnDTO accountLoginReturnDTO = new AccountReturnDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user),
                PhotoUrl = user.Photos?.FirstOrDefault(p => p.IsMain).Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
            return Ok(accountLoginReturnDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AccountReturnDTO>> Login(AccountLoginDTO loginDTO)
        {            
            var user = await _userManager.Users.Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.UserName == loginDTO.UserName.ToLower());
            if (user == null)
            {
                return BadRequest(Constants.LOGIN_USERNAME_INVALID);
            }    
            // Set photo url
            string photoUrl = "";
            if (user.Photos != null && user.Photos.Count > 0)
            {
                photoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: true);
            if (result.IsLockedOut)
                return BadRequest("Your account has been locked for maximun login try, please come back in 60 seconds.");
            if (!result.Succeeded) return BadRequest("Password is wrong.");
            // Set AccountLoginReturnDTO
            AccountReturnDTO accountLoginReturnDTO = new AccountReturnDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user),
                PhotoUrl = photoUrl,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
            return Ok(accountLoginReturnDTO);
        }

       
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());           
        }
    }
}
