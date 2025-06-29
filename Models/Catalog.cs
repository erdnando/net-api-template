using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netapi_template.Models
{
    [Table("Catalogs")]
    public class Catalog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Image { get; set; }
        
        [Column(TypeName = "decimal(3,2)")]
        public double Rating { get; set; } = 0;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; } = 0;
        
        public bool InStock { get; set; } = true;
        
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}