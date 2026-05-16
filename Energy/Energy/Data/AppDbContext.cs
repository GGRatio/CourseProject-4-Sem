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


        public async Task<List<Subscription>> GetSubscriptionsAsync()
        {
            return await Subscriptions.ToListAsync();
        }

        public async Task<Subscription> GetSubscriptionByIdAsync(int id)
        {
            return await Subscriptions.FindAsync(id);
        }

        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            await Subscriptions.AddAsync(subscription);
            await SaveChangesAsync();
        }

        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            Subscriptions.Update(subscription);
            await SaveChangesAsync();
        }

        public async Task DeleteSubscriptionAsync(int id)
        {
            var sub = await Subscriptions.FindAsync(id);
            if (sub != null)
            {
                Subscriptions.Remove(sub);
                await SaveChangesAsync();
            }
        }

    }
}
