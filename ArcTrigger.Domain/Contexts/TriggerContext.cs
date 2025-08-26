using Microsoft.EntityFrameworkCore;
using ArcTrigger.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace ArcTrigger.Domain.Contexts
{
    public class TriggerContext : DbContext
    {

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PreviousOrder> PreviousOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Default SQLite connection (creates ArcTrigger.db in app folder)
                optionsBuilder.UseSqlite("Data Source=ArcTrigger.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example configurations (you can extend these)
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Stock>().HasKey(s => s.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<PreviousOrder>().HasKey(po => po.Id);

            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Symbol)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<PreviousOrder>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(po => po.UserId);
        }
    }
}
