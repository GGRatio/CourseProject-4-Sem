using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Models
{       
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash {get; set;}
        public string Email { get; set;}

        //Доп информация
        public string? FirstName { get; set; }   
        public string? LastName { get; set; }    
        public string? Phone { get; set; }


        public string Role { get; set; } = "User";
        //Текущий Абонимент

        //Текущий Тренер 

        //Записи

    }
}
