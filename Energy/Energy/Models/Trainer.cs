using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Specialization { get; set; }

        public string Description { get; set; }

        public string PhotoUrl { get; set; }

        public int YearsOfExperience { get; set; }
    }
}
