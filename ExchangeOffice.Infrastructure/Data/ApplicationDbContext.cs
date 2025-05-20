using ExchangeOffice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExchangeOffice.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<RateHistory> RateHistories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Currency entity
            modelBuilder.Entity<Currency>()
                .HasIndex(c => c.Code)
                .IsUnique();

            // Configure RateHistory entity
            modelBuilder.Entity<RateHistory>()
                .HasOne(rh => rh.Currency)
                .WithMany(c => c.RateHistories)
                .HasForeignKey(rh => rh.CurrencyId);

            // Configure Customer entity
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.IdNumber)
                .IsUnique();

            // Configure Staff entity
            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Username)
                .IsUnique();

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.TransactionNumber)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.FromCurrency)
                .WithMany(c => c.FromTransactions)
                .HasForeignKey(t => t.FromCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ToCurrency)
                .WithMany(c => c.ToTransactions)
                .HasForeignKey(t => t.ToCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Staff)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.StaffId);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed initial currencies
            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Code = "USD",
                    Name = "US Dollar",
                    BuyRate = 0.95m,
                    SellRate = 1.05m,
                    LastUpdated = DateTime.Now
                },
                new Currency
                {
                    Id = 2,
                    Code = "EUR",
                    Name = "Euro",
                    BuyRate = 1.05m,
                    SellRate = 1.15m,
                    LastUpdated = DateTime.Now
                },
                new Currency
                {
                    Id = 3,
                    Code = "GBP",
                    Name = "British Pound",
                    BuyRate = 1.20m,
                    SellRate = 1.30m,
                    LastUpdated = DateTime.Now
                }
            );

            // Seed admin user
            modelBuilder.Entity<Staff>().HasData(
                new Staff
                {
                    Id = 1,
                    Username = "admin",
                    // This is a placeholder hash - in a real app, you'd use a proper password hasher
                    PasswordHash = "AQAAAAEAACcQAAAAEL0HFLxWTqX7zD9WJoIW7JKE7h1eBQEWZcuMYGFR8QQHz0/ZQCUNtYIl1H9vrjQCdA==", // Password: Admin123!
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Administrator",
                    IsActive = true
                }
            );
        }
    }
}
