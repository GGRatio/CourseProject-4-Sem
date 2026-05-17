using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class UserTrainer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TrainerId { get; set; }
        public DateTime SelectedDate { get; set; }

        public User User { get; set; }
        public Trainer Trainer { get; set; }
    }
}
