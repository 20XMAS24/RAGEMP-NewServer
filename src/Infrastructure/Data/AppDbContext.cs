using Microsoft.EntityFrameworkCore;
using Server.Core.Entities;

namespace Server.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Player> Players { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleModification> VehicleModifications { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<BankTransaction> BankTransactions { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                _connectionString,
                ServerVersion.AutoDetect(_connectionString),
                options => options
                    .CharSetBehavior(CharSetBehavior.NeverAppend)
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelaySeconds: 5, errorNumbersToAdd: null)
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Player
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(32);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.CharacterName).HasMaxLength(64);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.OwnedVehicles).WithOne(v => v.Owner).HasForeignKey(v => v.OwnerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.OwnedProperties).WithOne(p => p.Owner).HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.BankAccounts).WithOne(b => b.Owner).HasForeignKey(b => b.OwnerId).OnDelete(DeleteBehavior.Cascade);
        });

        // Vehicle
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Plate).IsRequired().HasMaxLength(16);
            entity.Property(e => e.Job).HasMaxLength(32);
            entity.HasIndex(e => e.Plate).IsUnique();
            entity.HasMany(e => e.Modifications).WithOne(m => m.Vehicle).HasForeignKey(m => m.VehicleId).OnDelete(DeleteBehavior.Cascade);
        });

        // Vehicle Modification
        modelBuilder.Entity<VehicleModification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ModName).IsRequired().HasMaxLength(100);
        });

        // Property
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PropertyType).IsRequired().HasMaxLength(32);
            entity.HasIndex(e => e.Address).IsUnique();
        });

        // Bank Account
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PIN).IsRequired();
            entity.Property(e => e.AccountType).IsRequired().HasMaxLength(32);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.HasMany(e => e.Transactions).WithOne(t => t.Account).HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Cascade);
        });

        // Bank Transaction
        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(32);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        // Job
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(64);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}