using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class ClassRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupClassId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsAttended { get; set; }  
        public bool IsCanceled { get; set; }  


        // Навигационные свойства
        public User User { get; set; }
        public GroupClass GroupClass { get; set; }
    }
}
