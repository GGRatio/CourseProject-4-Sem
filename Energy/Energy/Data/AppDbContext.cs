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
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //EF сам вызовет для подклюени к бд
        {
            optionsBuilder.UseSqlServer(
                "Server=LOST\\MSSQLSERVER06;Database=fitness_db;Trusted_Connection=True;Encrypt=False"
            );
        }
    }
}
