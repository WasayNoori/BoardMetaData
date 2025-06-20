using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardDataView.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public long MondayUserID { get; set; }
        
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;
        
        // Navigation properties can be added here if needed
    }
} 