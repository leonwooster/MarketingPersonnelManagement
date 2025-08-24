using Microsoft.EntityFrameworkCore;
using CompanyA.DataAccess.Models;

namespace CompanyA.DataAccess
{
    public class MarketingDbContext : DbContext
    {
        public MarketingDbContext(DbContextOptions<MarketingDbContext> options) : base(options)
        {
        }

        public DbSet<Personnel> Personnel { get; set; }
        public DbSet<CommissionProfile> CommissionProfiles { get; set; }
        public DbSet<Sales> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Personnel entity
            modelBuilder.Entity<Personnel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BankName).HasMaxLength(20);
                entity.Property(e => e.BankAccountNo).HasMaxLength(20);
                
                // Foreign key relationship
                entity.HasOne(p => p.CommissionProfile)
                      .WithMany(cp => cp.Personnel)
                      .HasForeignKey(p => p.CommissionProfileId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Check constraints (using modern syntax)
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Personnel_Age", "age >= 19");
                    t.HasCheckConstraint("CK_Personnel_Name", "LEN(LTRIM(RTRIM(name))) > 0");
                    t.HasCheckConstraint("CK_Personnel_Phone", "LEN(LTRIM(RTRIM(phone))) > 0");
                });
            });

            // Configure CommissionProfile entity
            modelBuilder.Entity<CommissionProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommissionFixed).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CommissionPercentage).HasColumnType("decimal(10,6)");
                
                // Check constraints (using modern syntax)
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_CommissionProfile_Fixed", "commission_fixed >= 0");
                    t.HasCheckConstraint("CK_CommissionProfile_Percentage", "commission_percentage >= 0 AND commission_percentage <= 1");
                });
            });

            // Configure Sales entity
            modelBuilder.Entity<Sales>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SalesAmount).HasColumnType("decimal(10,2)");
                
                // Foreign key relationship with cascade delete
                entity.HasOne(s => s.Personnel)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(s => s.PersonnelId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Check constraints (using modern syntax)
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Sales_Amount", "sales_amount >= 0");
                    t.HasCheckConstraint("CK_Sales_Date", "report_date <= GETDATE()");
                });

                // Indexes
                entity.HasIndex(e => e.PersonnelId).HasDatabaseName("IX_Sales_PersonnelId");
                entity.HasIndex(e => e.ReportDate).HasDatabaseName("IX_Sales_ReportDate");
            });

            // Additional indexes
            modelBuilder.Entity<Personnel>()
                .HasIndex(e => e.CommissionProfileId)
                .HasDatabaseName("IX_Personnel_CommissionProfileId");

            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Commission Profiles
            modelBuilder.Entity<CommissionProfile>().HasData(
                new CommissionProfile { Id = 1, ProfileName = 1, CommissionFixed = 500.00m, CommissionPercentage = 0.050000m },
                new CommissionProfile { Id = 2, ProfileName = 2, CommissionFixed = 750.00m, CommissionPercentage = 0.030000m },
                new CommissionProfile { Id = 3, ProfileName = 3, CommissionFixed = 300.00m, CommissionPercentage = 0.080000m }
            );

            // Seed Personnel
            modelBuilder.Entity<Personnel>().HasData(
                new Personnel { Id = 1, Name = "John Smith", Age = 25, Phone = "555-0101", CommissionProfileId = 1, BankName = "Chase Bank", BankAccountNo = "1234567890" },
                new Personnel { Id = 2, Name = "Sarah Johnson", Age = 28, Phone = "555-0102", CommissionProfileId = 2, BankName = "Wells Fargo", BankAccountNo = "2345678901" },
                new Personnel { Id = 3, Name = "Michael Brown", Age = 32, Phone = "555-0103", CommissionProfileId = 1, BankName = "Bank of America", BankAccountNo = "3456789012" },
                new Personnel { Id = 4, Name = "Emily Davis", Age = 24, Phone = "555-0104", CommissionProfileId = 3, BankName = "Citibank", BankAccountNo = "4567890123" },
                new Personnel { Id = 5, Name = "David Wilson", Age = 29, Phone = "555-0105", CommissionProfileId = 2, BankName = "TD Bank", BankAccountNo = "5678901234" }
            );

            // Seed Sales (sample data with static dates)
            modelBuilder.Entity<Sales>().HasData(
                new Sales { Id = 1, PersonnelId = 1, ReportDate = new DateTime(2025, 7, 15), SalesAmount = 1250.00m },
                new Sales { Id = 2, PersonnelId = 1, ReportDate = new DateTime(2025, 7, 20), SalesAmount = 980.50m },
                new Sales { Id = 3, PersonnelId = 2, ReportDate = new DateTime(2025, 7, 18), SalesAmount = 2150.00m },
                new Sales { Id = 4, PersonnelId = 2, ReportDate = new DateTime(2025, 7, 22), SalesAmount = 1875.50m },
                new Sales { Id = 5, PersonnelId = 3, ReportDate = new DateTime(2025, 7, 25), SalesAmount = 950.00m }
            );
        }
    }
}
