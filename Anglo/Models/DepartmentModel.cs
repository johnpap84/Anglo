using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Models
{
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(30, ErrorMessage = "Department name cannot be longer than 30 characters")]
        [DisplayName("Department name")]
        public string DepartmentName { get; set; }

        public int NumberOfJobTitles { get; set; }
        public int NumberOfEmployees { get; set; }

        public List<JobTitleModel> JobTitles { get; set; }

        public List<EmployeeModel> EmployeesInDepartment { get; set; }
    }
}
