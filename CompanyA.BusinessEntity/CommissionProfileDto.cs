using System.ComponentModel.DataAnnotations;

namespace CompanyA.BusinessEntity
{
    public class CommissionProfileDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Profile name is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Profile name must be a positive integer")]
        public int ProfileName { get; set; }

        [Required(ErrorMessage = "Commission fixed amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Commission fixed amount must be non-negative")]
        public decimal CommissionFixed { get; set; }

        [Required(ErrorMessage = "Commission percentage is required")]
        [Range(0, 1, ErrorMessage = "Commission percentage must be between 0 and 1")]
        public decimal CommissionPercentage { get; set; }
    }
}