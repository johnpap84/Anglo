using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Models
{
    public class JobTitleModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Job Title's name is required")]
        [StringLength(30, ErrorMessage = "Job Title's name cannot be longer than 30 characters")]
        [DisplayName("Job Title name")]
        public string JobTitleName { get; set; }

        [Required(ErrorMessage = "Job Title needs to be set to a Department")]
        public string InDepartment { get; set; }

        public int numberOfEmployees { get; set; }

        public List<EmployeeModel> EmployeesInJob { get; set; }
    }
}
