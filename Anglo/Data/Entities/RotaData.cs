using System;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Data.Entities
{
    public class RotaData
    {
        [Key]
        public int RotaId { get; set; }

        [Required]
        public Employee RotaForEmployee { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Shift Shift { get; set; }
        
    }
}
