using AngloRota.Shared;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Data.Entities
{
    public class Shift
    {
        [Key]
        public int ShiftId { get; set; }

        [Required]
        [MaxLength(30)]
        private string _shiftName;
        public string ShiftName
        {
            get { return _shiftName; }
            set { _shiftName = value.FirstLettersToUpperCase(); }
        }

        [Required]
        public int DurationInMins { get; set; }
    }
}
