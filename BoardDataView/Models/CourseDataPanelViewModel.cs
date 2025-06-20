using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BoardDataView.Models
{
    public class CourseDataPanelViewModel
    {
        [Display(Name = "Course Name")]
        public int? SelectedCourseId { get; set; }
        
        [Display(Name = "Version Title")]
        public int? SelectedVersionId { get; set; }
        
        [Display(Name = "Product")]
        public int? SelectedProductId { get; set; }
        
        [Display(Name = "Language")]
        [StringLength(10)]
        public string? SelectedLanguage { get; set; }
        
        [Display(Name = "Course Instructor")]
        public int? SelectedInstructorId { get; set; }
        
        [Display(Name = "Version Flip")]
        public bool VersionFlip { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        // Dropdown data
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CourseVersions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Languages { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Instructors { get; set; } = new List<SelectListItem>();
        
        // Control visibility
        public bool ShowDataPanel { get; set; } = false;
    }
} 