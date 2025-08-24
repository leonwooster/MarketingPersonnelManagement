using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyA.DataAccess.Models
{
    [Table("CommissionProfile")]
    public class CommissionProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("profile_name")]
        public int ProfileName { get; set; }

        [Required]
        [Column("commission_fixed", TypeName = "decimal(10,2)")]
        public decimal CommissionFixed { get; set; }

        [Required]
        [Column("commission_percentage", TypeName = "decimal(10,6)")]
        public decimal CommissionPercentage { get; set; }

        // Navigation properties
        public virtual ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
    }
}