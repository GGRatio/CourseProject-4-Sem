using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Subscriptionid { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        // Навигационные свойства
        public User User { get; set; }
        public Subscription Subscription { get; set; }

    }
}
