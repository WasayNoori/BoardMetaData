using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoardDataView.Data;
using BoardDataView.Models;
using Microsoft.Extensions.Logging;

namespace BoardDataView.Pages
{
    public class BoardMetaModel : PageModel
    {
        private readonly SPAutomationDbContext _context;
        private readonly ILogger<BoardMetaModel> _logger;

        public BoardMetaModel(SPAutomationDbContext context, ILogger<BoardMetaModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public CourseInstanceViewModel DataPanel { get; set; } = new CourseInstanceViewModel();

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(long? boardId = null)
        {
            await LoadDropdownDataAsync();
            
            // Always show the data panel
            DataPanel.ShowDataPanel = true;
            
            if (boardId.HasValue)
            {
                await LoadCourseInstanceAsync(boardId.Value);
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            await LoadDropdownDataAsync();
            
            // Always show the data panel
            DataPanel.ShowDataPanel = true;
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (DataPanel.IsNewRecord)
                {
                    await CreateCourseInstanceAsync();
                }
                else
                {
                    await UpdateCourseInstanceAsync();
                }
                
                DataPanel.IsEditMode = false;
                await LoadCourseInstanceAsync(DataPanel.BoardId!.Value);
                
                _logger.LogInformation("CourseInstance saved successfully for BoardId: {BoardId}", DataPanel.BoardId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving CourseInstance");
                ErrorMessage = "An error occurred while saving. Please try again.";
            }
            
            return Page();
        }



        public async Task<IActionResult> OnPostToggleEditModeAsync()
        {
            await LoadDropdownDataAsync();
            
            // Always show the data panel
            DataPanel.ShowDataPanel = true;
            
            if (!DataPanel.IsNewRecord && DataPanel.BoardId.HasValue)
            {
                // For existing records, toggle edit mode
                await LoadCourseInstanceAsync(DataPanel.BoardId.Value);
                DataPanel.IsEditMode = !DataPanel.IsEditMode;
            }
            else if (DataPanel.IsNewRecord && !DataPanel.IsEditMode)
            {
                // For new records, set up for editing (don't toggle)
                DataPanel.IsEditMode = true;
                DataPanel.IsNewRecord = true;
                DataPanel.BoardId = null;
                // Clear any existing data
                DataPanel.SelectedCourseId = null;
                DataPanel.VersionTitle = null;
                DataPanel.SelectedLanguageId = null;
                DataPanel.SelectedInstructorId = null;
                DataPanel.SelectedProductId = null;
                DataPanel.Description = null;
            }
            else if (DataPanel.IsNewRecord && DataPanel.IsEditMode)
            {
                // Cancel new record creation
                DataPanel.IsEditMode = false;
            }
            
            return Page();
        }

        private async Task LoadCourseInstanceAsync(long boardId)
        {
            try
            {
                var courseInstance = await _context.CourseInstances
                    .Include(ci => ci.Course)
                    .Include(ci => ci.Language)
                    .Include(ci => ci.Instructor)
                    .Include(ci => ci.Product)
                    .FirstOrDefaultAsync(ci => ci.BoardId == boardId);

                if (courseInstance != null)
                {
                    DataPanel.BoardId = courseInstance.BoardId;
                    DataPanel.SelectedCourseId = courseInstance.CourseId;
                    DataPanel.VersionTitle = courseInstance.VersionTitle;
                    DataPanel.SelectedLanguageId = courseInstance.LanguageID;
                    DataPanel.SelectedInstructorId = courseInstance.InstructorId;
                    DataPanel.SelectedProductId = courseInstance.ProductId;
                    DataPanel.Description = courseInstance.Description;
                    
                    // Set display names for read-only mode
                    DataPanel.CourseDisplayName = courseInstance.Course?.Name ?? "Unknown Course";
                    DataPanel.LanguageDisplayName = courseInstance.Language?.LanguageName ?? "Unknown Language";
                    DataPanel.InstructorDisplayName = courseInstance.Instructor?.FullName ?? "Unknown Instructor";
                    DataPanel.ProductDisplayName = courseInstance.Product?.ProductName ?? "Unknown Product";
                    
                    DataPanel.IsNewRecord = false;
                }
                else
                {
                    DataPanel.BoardId = boardId;
                    DataPanel.IsNewRecord = true;
                    DataPanel.IsEditMode = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CourseInstance for BoardId: {BoardId}", boardId);
                ErrorMessage = "Error loading course instance data.";
            }
        }

        private async Task CreateCourseInstanceAsync()
        {
            var courseInstance = new CourseInstance
            {
                BoardId = DataPanel.BoardId!.Value,
                CourseId = DataPanel.SelectedCourseId!.Value,
                VersionTitle = DataPanel.VersionTitle,
                LanguageID = DataPanel.SelectedLanguageId,
                InstructorId = DataPanel.SelectedInstructorId,
                ProductId = DataPanel.SelectedProductId,
                Description = DataPanel.Description
            };

            _context.CourseInstances.Add(courseInstance);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        private async Task UpdateCourseInstanceAsync()
        {
            var courseInstance = await _context.CourseInstances
                .FirstOrDefaultAsync(ci => ci.BoardId == DataPanel.BoardId!.Value);

            if (courseInstance != null)
            {
                courseInstance.CourseId = DataPanel.SelectedCourseId!.Value;
                courseInstance.VersionTitle = DataPanel.VersionTitle;
                courseInstance.LanguageID = DataPanel.SelectedLanguageId;
                courseInstance.InstructorId = DataPanel.SelectedInstructorId;
                courseInstance.ProductId = DataPanel.SelectedProductId;
                courseInstance.Description = DataPanel.Description;

                await _context.SaveChangesAsync();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                _logger.LogInformation("Starting to load dropdown data");

                // Initialize lists to prevent null reference errors
                DataPanel.Courses = new List<SelectListItem>();
                DataPanel.Languages = new List<SelectListItem>();
                DataPanel.Products = new List<SelectListItem>();
                DataPanel.Instructors = new List<SelectListItem>();

                // Load courses with null safety
                try
                {
                    var courses = await _context.Courses
                        .Where(c => c.Name != null && c.Name != "")
                        .Select(c => new SelectListItem
                        {
                            Value = c.CourseId.ToString(),
                            Text = c.Name ?? $"Course {c.CourseId}"
                        })
                        .ToListAsync();
                    
                    DataPanel.Courses = courses;
                    _logger.LogInformation("Loaded {Count} courses", courses.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading courses");
                    DataPanel.Courses = new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "Error loading courses" } 
                    };
                }

                // Load languages with null safety
                try
                {
                    var languages = await _context.Languages
                        .Where(l => l.LanguageName != null && l.LanguageName != "")
                        .Select(l => new SelectListItem
                        {
                            Value = l.LanguageID.ToString(),
                            Text = l.LanguageName ?? "Unknown Language"
                        })
                        .ToListAsync();
                    
                    DataPanel.Languages = languages;
                    _logger.LogInformation("Loaded {Count} languages", languages.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading languages");
                    DataPanel.Languages = new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "Error loading languages" } 
                    };
                }

                // Load products with null safety
                try
                {
                    var products = await _context.Products
                        .Where(p => p.ProductName != null && p.ProductName != "")
                        .Select(p => new SelectListItem
                        {
                            Value = p.ProductID.ToString(),
                            Text = p.ProductName ?? $"Product {p.ProductID}"
                        })
                        .ToListAsync();
                    
                    DataPanel.Products = products;
                    _logger.LogInformation("Loaded {Count} products", products.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading products");
                    DataPanel.Products = new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "Error loading products" } 
                    };
                }

                // Load instructors with null safety
                try
                {
                    _logger.LogInformation("Attempting to load instructors from Users table");
                    
                    // First, let's see if we can get ANY users
                    var allUsers = await _context.Users.ToListAsync();
                    _logger.LogInformation("Found {Count} total users in database", allUsers.Count);
                    
                    // Now filter for instructors
                    var instructors = allUsers
                        .Where(u => !string.IsNullOrEmpty(u.FullName))
                        .Select(u => new SelectListItem
                        {
                            Value = u.MondayUserID.ToString(),
                            Text = u.FullName ?? $"User {u.MondayUserID}"
                        })
                        .ToList();
                    
                    DataPanel.Instructors = instructors;
                    _logger.LogInformation("Loaded {Count} instructors after filtering", instructors.Count);
                    
                    // Log the first few instructors for debugging
                    foreach (var instructor in instructors.Take(3))
                    {
                        _logger.LogInformation("Instructor: {Value} - {Text}", instructor.Value, instructor.Text);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading instructors");
                    DataPanel.Instructors = new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "Error loading instructors" } 
                    };
                }

                _logger.LogInformation("Completed loading dropdown data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in LoadDropdownDataAsync");
                ErrorMessage = "Unable to load data from the database. Please check your connection and try again.";
                
                // Ensure all lists are initialized even on error
                DataPanel.Courses ??= new List<SelectListItem>();
                DataPanel.Languages ??= new List<SelectListItem>();
                DataPanel.Products ??= new List<SelectListItem>();
                DataPanel.Instructors ??= new List<SelectListItem>();
            }
        }
    }
}
