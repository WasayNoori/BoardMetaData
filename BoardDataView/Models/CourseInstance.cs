using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardDataView.Models
{
    [Table("CourseInstances")]
    public class CourseInstance
    {
        [Key]
        public long BoardId { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        
        [StringLength(100)]
        public string? VersionTitle { get; set; }
        
        [Column("LanguageID")]
        public int? LanguageID { get; set; }
        
        public long? InstructorId { get; set; }
        
        public int? ProductId { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
        
        [ForeignKey("InstructorId")]
        public virtual User? Instructor { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        
        [ForeignKey("LanguageID")]
        public virtual Language? Language { get; set; }
    }
}
