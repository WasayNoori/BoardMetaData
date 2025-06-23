using Microsoft.AspNetCore.Mvc;
using BoardDataView.Data;
using BoardDataView.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardDataView.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This means the URL will be /api/API
    public class APIController : ControllerBase
    {
        private readonly SPAutomationDbContext _context;
        private readonly IConfiguration _configuration;

        // The DbContext and Configuration are injected here
        public APIController(SPAutomationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Get board ID from configuration (for development/testing only)
        [HttpGet("boardid")]
        public IActionResult GetBoardId()
        {
            try
            {
                // Only provide test board ID in Development environment
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment != "Development")
                {
                    return BadRequest(new { message = "Board ID API only available in Development environment. In production, board ID should come from Monday.com SDK." });
                }

                var boardId = _configuration["TestSettings:BoardId"];
                if (string.IsNullOrEmpty(boardId))
                {
                    return NotFound(new { message = "Board ID not configured in development settings" });
                }
                
                return Ok(new { boardId = boardId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving board ID" });
            }
        }

        // This method responds to HTTP GET requests to /api/API/users
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

        // Get CourseInstance by BoardId
        [HttpGet("courseinstance/{boardId}")]
        public async Task<IActionResult> GetCourseInstance(long boardId)
        {
            try
            {
                var courseInstance = await _context.CourseInstances
                    .Include(ci => ci.Course)
                    .Include(ci => ci.Language)
                    .Include(ci => ci.Instructor)
                    .Include(ci => ci.Product)
                    .Where(ci => ci.BoardId == boardId)
                    .Select(ci => new CourseInstanceApiViewModel
                    {
                        BoardId = ci.BoardId,
                        CourseId = ci.CourseId,
                        CourseName = ci.Course.Name,
                        VersionTitle = ci.VersionTitle,
                        LanguageId = ci.LanguageID,
                        LanguageName = ci.Language != null ? ci.Language.LanguageName : null,
                        InstructorId = ci.InstructorId,
                        InstructorName = ci.Instructor != null ? ci.Instructor.FullName : null,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product != null ? ci.Product.ProductName : null,
                        Description = ci.Description,
                        Lessons = ci.Lessons,
                        FileLocation = ci.FileLocation,
                        RevisionFlip = ci.RevisionFlip ?? false
                    })
                    .FirstOrDefaultAsync();

                if (courseInstance == null)
                {
                    return NotFound($"No course instance found for BoardId: {boardId}");
                }

                return Ok(courseInstance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching course instance.");
            }
        }

        // Get dropdown data for forms
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var courses = await _context.Courses
                    .Where(c => !string.IsNullOrEmpty(c.Name))
                    .Select(c => new { value = c.CourseId, text = c.Name })
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching courses.");
            }
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var languages = await _context.Languages
                    .Where(l => !string.IsNullOrEmpty(l.LanguageName))
                    .Select(l => new { value = l.LanguageID, text = l.LanguageName })
                    .ToListAsync();

                return Ok(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching languages.");
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => !string.IsNullOrEmpty(p.ProductName))
                    .Select(p => new { value = p.ProductID, text = p.ProductName })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching products.");
            }
        }

        // Save or update course instance
        [HttpPost("courseinstance")]
        public async Task<IActionResult> SaveCourseInstance([FromBody] CourseInstanceApiViewModel model)
        {
            try
            {
                // Check if a course instance already exists for this BoardId
                var existingCourseInstance = await _context.CourseInstances
                    .FirstOrDefaultAsync(ci => ci.BoardId == model.BoardId);

                if (existingCourseInstance != null)
                {
                    // Update existing record
                    existingCourseInstance.CourseId = model.CourseId ?? 0;
                    existingCourseInstance.VersionTitle = model.VersionTitle;
                    existingCourseInstance.LanguageID = model.LanguageId;
                    existingCourseInstance.InstructorId = model.InstructorId;
                    existingCourseInstance.ProductId = model.ProductId;
                    existingCourseInstance.Description = model.Description;
                    existingCourseInstance.Lessons = model.Lessons;
                    existingCourseInstance.FileLocation = model.FileLocation;
                    existingCourseInstance.RevisionFlip = model.RevisionFlip;

                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "Course instance updated successfully!", isNew = false });
                }
                else
                {
                    // Create new record
                    var newCourseInstance = new CourseInstance
                    {
                        BoardId = model.BoardId,
                        CourseId = model.CourseId ?? 0,
                        VersionTitle = model.VersionTitle,
                        LanguageID = model.LanguageId,
                        InstructorId = model.InstructorId,
                        ProductId = model.ProductId,
                        Description = model.Description,
                        Lessons = model.Lessons,
                        FileLocation = model.FileLocation,
                        RevisionFlip = model.RevisionFlip,
                        CreatedAt = DateTime.Now
                    };

                    _context.CourseInstances.Add(newCourseInstance);
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "Course instance created successfully!", isNew = true });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving the course instance." });
            }
        }

        // Delete course instance
        [HttpDelete("courseinstance/{boardId}")]
        public async Task<IActionResult> DeleteCourseInstance(long boardId)
        {
            try
            {
                var courseInstance = await _context.CourseInstances
                    .FirstOrDefaultAsync(ci => ci.BoardId == boardId);

                if (courseInstance == null)
                {
                    return NotFound(new { success = false, message = "Course instance not found." });
                }

                _context.CourseInstances.Remove(courseInstance);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Course instance deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the course instance." });
            }
        }
    }
}
