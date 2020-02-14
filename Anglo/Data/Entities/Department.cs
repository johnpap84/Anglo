using AngloRota.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Data.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(30)]
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value.FirstLettersToUpperCase(); ; }
        }

        public List<JobTitle> JobTitles { get; set; }

        public List<Employee> EmployeesInDepartment { get; set; }

    }
}
