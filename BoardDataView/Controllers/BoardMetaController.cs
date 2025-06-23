using BoardDataView.Data;
using BoardDataView.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardDataView.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This means the URL will be /api/BoardMeta
    public class BoardMetaController : ControllerBase
    {
        private readonly SPAutomationDbContext _context;

        // The DbContext is injected here, just like in the Razor Page
        public BoardMetaController(SPAutomationDbContext context)
        {
            _context = context;
        }

        // This method responds to HTTP GET requests to /api/BoardMeta/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.FullName))
                    .Select(u => new UserViewModel // Projecting to our simple ViewModel
                    {
                        UserId = u.MondayUserID,
                        FullName = u.FullName
                    })
                    .ToListAsync();

                return Ok(users); // Return a 200 OK status with the list of users as JSON
            }
            catch (Exception ex)
            {
                // In case of a database error, return a 500 Internal Server Error
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }
    }
} 