using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardDataView.Models
{
    [Table("Languages")]
    public class Language
    {
        [Key]
        public int LanguageID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LanguageName { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ICollection<CourseInstance> CourseInstances { get; set; } = new List<CourseInstance>();
    }
}
