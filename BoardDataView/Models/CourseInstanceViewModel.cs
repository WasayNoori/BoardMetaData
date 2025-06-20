using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BoardDataView.Models
{
    public class CourseInstanceViewModel
    {
        [Display(Name = "Board ID")]
        public long? BoardId { get; set; }
        
        [Display(Name = "Course")]
        public int? SelectedCourseId { get; set; }
        
        [Display(Name = "Version Title")]
        [StringLength(100)]
        public string? VersionTitle { get; set; }
        
        [Display(Name = "Language")]
        public int? SelectedLanguageId { get; set; }
        
        [Display(Name = "Instructor")]
        public long? SelectedInstructorId { get; set; }
        
        [Display(Name = "Product")]
        public int? SelectedProductId { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        // Display values for read-only mode
        public string? CourseDisplayName { get; set; }
        public string? LanguageDisplayName { get; set; }
        public string? InstructorDisplayName { get; set; }
        public string? ProductDisplayName { get; set; }
        
        // Dropdown data
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Languages { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Instructors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        
        // Control states
        public bool IsEditMode { get; set; } = false;
        public bool ShowDataPanel { get; set; } = false;
        public bool IsNewRecord { get; set; } = true;
    }
} 