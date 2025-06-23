namespace BoardDataView.Models
{
    public class CourseInstanceApiViewModel
    {
        public long BoardId { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? VersionTitle { get; set; }
        public int? LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public long? InstructorId { get; set; }
        public string? InstructorName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
    }
} 