using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyA.DataAccess.Models
{
    [Table("Sales")]
    public class Sales
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("personnel_id")]
        public int PersonnelId { get; set; }

        [Required]
        [Column("report_date")]
        public DateTime ReportDate { get; set; }

        [Required]
        [Column("sales_amount", TypeName = "decimal(10,2)")]
        public decimal SalesAmount { get; set; }

        // Navigation properties
        public virtual Personnel Personnel { get; set; } = null!;
    }
}