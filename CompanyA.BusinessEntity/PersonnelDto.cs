using System.ComponentModel.DataAnnotations;

namespace CompanyA.BusinessEntity
{
    public class PersonnelDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(19, int.MaxValue, ErrorMessage = "Age must be 19 or older")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Commission profile is required")]
        public int CommissionProfileId { get; set; }

        [StringLength(20, ErrorMessage = "Bank name cannot exceed 20 characters")]
        public string? BankName { get; set; }

        [StringLength(20, ErrorMessage = "Bank account number cannot exceed 20 characters")]
        public string? BankAccountNo { get; set; }
    }
}