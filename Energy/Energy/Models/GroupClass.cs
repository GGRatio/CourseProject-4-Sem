using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class GroupClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }  

        [Required]
        public string Instructor { get; set; }  

        public string Description { get; set; }

        [Required]
        public DateTime ClassDate { get; set; }  

        [Required]
        public int MaxParticipants { get; set; } 

        public int CurrentParticipants { get; set; }  

        public int DurationMinutes { get; set; }

        public string ClassDateText => ClassDate.ToString("dd.MM.yyyy HH:mm");
    }
}
