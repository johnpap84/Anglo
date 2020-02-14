using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Models
{
    public class RotaDataModel
    {
        public int RotaId { get; set; }

        [Required(ErrorMessage = "Employee's id is required")]
        [DisplayName("Employee's Id")]
        public int EmployeeId { get; set; }

        [DisplayName("Employee's name")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Shift needs to be set to a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Shift name is required")]
        [DisplayName("Shift name")]
        public string ShiftName { get; set; }

        [DisplayName("Duration of the Shift")]
        public int DurationInMins { get; set; }
    }
}
