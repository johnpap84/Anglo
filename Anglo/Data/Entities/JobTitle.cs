using AngloRota.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Data.Entities
{
    public class JobTitle
    {
        [Key]
        public int JobTitleId { get; set; }

        [Required]
        [StringLength(30)]
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value.FirstLettersToUpperCase(); }
        }

        [Required]
        public Department Department { get; set; }

        public List<Employee> EmployeesInJob { get; set; }
    }
}
