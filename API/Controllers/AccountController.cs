using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using API.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AccountReturnDTO>> Register([FromBody] AccountRegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
            {
                return BadRequest(Constants.REGISTER_USERNAME_EXIST);
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            // Set AccountLoginReturnDTO
            AccountReturnDTO accountLoginReturnDTO = new AccountReturnDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            return Ok(accountLoginReturnDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AccountReturnDTO>> Login(AccountLoginDTO loginDTO)
        {            
            var user = await _context.AppUsers.SingleOrDefaultAsync(u => u.UserName == loginDTO.UserName.ToLower());
            if (user == null)
            {
                return BadRequest(Constants.LOGIN_USERNAME_INVALID);
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var hashPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for (int i = 0; i < hashPassword.Length; i++)
            {
                if (user.PasswordHash[i] != hashPassword[i])
                {
                    return BadRequest(Constants.LOGIN_PASSWORD_INVALID);
                }
            }
            // Set AccountLoginReturnDTO
            AccountReturnDTO accountLoginReturnDTO = new AccountReturnDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            return Ok(accountLoginReturnDTO);
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName == username.ToLower());           
        }
    }
}
