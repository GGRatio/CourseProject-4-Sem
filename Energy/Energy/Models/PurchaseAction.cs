using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{
    public class PurchaseAction
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public int Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int DurationDays { get; set; }
    }
}
