using AngloRota.Shared;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Data.Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        
        [Required]
        private string _employeeName;
        public string EmployeeName
        {
            get { return _employeeName; }
            set { _employeeName = value.FirstLettersToUpperCase(); }
        }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        [Required]
        public Department Department { get; set; }
        
        [Required]
        public JobTitle JobTitle { get; set; }

        public int HolidayQuota { get; set; }

    }
}
