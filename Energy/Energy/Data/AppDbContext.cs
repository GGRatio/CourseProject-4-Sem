using Energy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //EF сам вызовет для подклюени к бд
        {
            optionsBuilder.UseMySql("Server=localhost;Database=fitness_db;Uid=root;Pwd=;", 
                new MySqlServerVersion(new Version(8, 0, 28))
            );
        }
    }
}
