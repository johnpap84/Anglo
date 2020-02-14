using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Models
{
    public class ShiftModel
    {
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "Shift name is required")]
        [StringLength(20, ErrorMessage = "Shift name cannot be longer than 20 characters")]
        [DisplayName("Shift name")]
        public string ShiftName { get; set; }

        [Required(ErrorMessage = "The duration of the Shift needs to be set up")]
        [DisplayName("Duration of the Shift")]
        [Range(0, 1920, ErrorMessage = "Duration of the Shift must be between 0 and 32 hours")]
        public int DurationInMins { get; set; }
    }
}
