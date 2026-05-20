using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Helpers
{
    public class Session
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserLogin { get; set; }
        public static string CurrentUserRole { get; set; }

        public static string CurrentUserFirstName { get; set; }  
        public static string CurrentUserLastName { get; set; }   
    }
}
