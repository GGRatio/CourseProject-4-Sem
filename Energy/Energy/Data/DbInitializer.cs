using Energy.Helpers;
using Energy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Data
{
    class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                context.Users.Add(new User
                {
                    Login = "admin",
                    PasswordHash = PasswordHelper.HashPassword("admin"),
                    Email = "admin@fit.com",
                    Role = "Admin",
                    FirstName = "Admin",
                    LastName = "Admin"
                });
                context.SaveChanges();
            }
        }
    }
}
