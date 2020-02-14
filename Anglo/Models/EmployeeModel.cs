using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Models
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee's name is required")]
        [DisplayName("Employee's name")]
        public string Name { get; set; }

        [DisplayName("Email address")]
        public string Email { get; set; }
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [DisplayName("Department")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Job Title is required")]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

        public int HolidayQuota { get; set; }
    }
}
