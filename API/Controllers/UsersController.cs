using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{    
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]        
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {
            var users =await _context.AppUsers.ToListAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetAppUser(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
    }
}
