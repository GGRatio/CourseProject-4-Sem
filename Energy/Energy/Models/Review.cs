using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TrainerId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Trainer Trainer { get; set; }
    }
}