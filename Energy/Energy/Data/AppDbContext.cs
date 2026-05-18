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
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<UserTrainer> UserTrainers { get; set; }

        public DbSet<GroupClass> GroupClasses { get; set; }
        public DbSet<ClassRegistration> ClassRegistrations { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //EF сам вызовет для подклюени к бд
        {
            optionsBuilder.UseSqlServer(
                "Server=LOST\\MSSQLSERVER06;Database=Energy_DB;Trusted_Connection=True;Encrypt=False"
            );
        }


    }
}
