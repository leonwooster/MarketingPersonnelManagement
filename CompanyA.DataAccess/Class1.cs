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

                // Check constraints (handled by database)
                entity.HasCheckConstraint("CK_Personnel_Age", "age >= 19");
                entity.HasCheckConstraint("CK_Personnel_Name", "LEN(LTRIM(RTRIM(name))) > 0");
                entity.HasCheckConstraint("CK_Personnel_Phone", "LEN(LTRIM(RTRIM(phone))) > 0");
            });

            // Configure CommissionProfile entity
            modelBuilder.Entity<CommissionProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommissionFixed).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CommissionPercentage).HasColumnType("decimal(10,6)");
                
                // Check constraints
                entity.HasCheckConstraint("CK_CommissionProfile_Fixed", "commission_fixed >= 0");
                entity.HasCheckConstraint("CK_CommissionProfile_Percentage", "commission_percentage >= 0 AND commission_percentage <= 1");
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

                // Check constraints
                entity.HasCheckConstraint("CK_Sales_Amount", "sales_amount >= 0");
                entity.HasCheckConstraint("CK_Sales_Date", "report_date <= GETDATE()");

                // Indexes
                entity.HasIndex(e => e.PersonnelId).HasDatabaseName("IX_Sales_PersonnelId");
                entity.HasIndex(e => e.ReportDate).HasDatabaseName("IX_Sales_ReportDate");
            });

            // Additional indexes
            modelBuilder.Entity<Personnel>()
                .HasIndex(e => e.CommissionProfileId)
                .HasDatabaseName("IX_Personnel_CommissionProfileId");
        }
    }
}
