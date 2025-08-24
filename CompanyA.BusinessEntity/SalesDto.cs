using System.ComponentModel.DataAnnotations;

namespace CompanyA.BusinessEntity
{
    public class SalesDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Personnel ID is required")]
        public int PersonnelId { get; set; }

        [Required(ErrorMessage = "Report date is required")]
        public DateTime ReportDate { get; set; }

        [Required(ErrorMessage = "Sales amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Sales amount must be non-negative")]
        public decimal SalesAmount { get; set; }
    }
}