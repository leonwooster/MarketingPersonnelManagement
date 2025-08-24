using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyA.DataAccess.Models
{
    [Table("Personnel")]
    public class Personnel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("age")]
        public int Age { get; set; }

        [Required]
        [StringLength(20)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Column("commission_profile_id")]
        public int CommissionProfileId { get; set; }

        [StringLength(20)]
        [Column("bank_name")]
        public string? BankName { get; set; }

        [StringLength(20)]
        [Column("bank_account_no")]
        public string? BankAccountNo { get; set; }

        // Navigation properties
        public virtual CommissionProfile CommissionProfile { get; set; } = null!;
        public virtual ICollection<Sales> Sales { get; set; } = new List<Sales>();
    }
}