using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Helpers
{
    [Serializable]
    public class SavedUser
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
