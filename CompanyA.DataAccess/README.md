# CompanyA.DataAccess

Data access layer for the Marketing Personnel Management System using Entity Framework Core.

## Overview

This class library provides data access functionality using Entity Framework Core with repository pattern implementation. Handles database operations for Personnel, Sales, and CommissionProfile entities.

## Technology Stack

- **ORM**: Entity Framework Core 6.0+
- **Database**: SQL Server 2016+
- **Pattern**: Repository + Unit of Work
- **Migrations**: Code-First approach

## Database Context

### MarketingDbContext
```csharp
public class MarketingDbContext : DbContext
{
    public MarketingDbContext(DbContextOptions<MarketingDbContext> options) : base(options) { }
    
    public DbSet<Personnel> Personnel { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<CommissionProfile> CommissionProfile { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships and constraints
        ConfigurePersonnel(modelBuilder);
        ConfigureSales(modelBuilder);
        ConfigureCommissionProfile(modelBuilder);
    }
}
```

## Entity Configuration

### Personnel Configuration
```csharp
private void ConfigurePersonnel(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Personnel>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");
            
        entity.Property(e => e.Age)
            .IsRequired()
            .HasColumnName("age");
            
        entity.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("phone");
            
        entity.Property(e => e.CommissionProfileId)
            .IsRequired()
            .HasColumnName("commission_profile_id");
            
        entity.Property(e => e.BankName)
            .HasMaxLength(20)
            .HasColumnName("bank_name");
            
        entity.Property(e => e.BankAccountNo)
            .HasMaxLength(20)
            .HasColumnName("bank_account_no");
            
        // Foreign key relationship
        entity.HasOne(e => e.CommissionProfile)
            .WithMany(cp => cp.Personnel)
            .HasForeignKey(e => e.CommissionProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    });
}
```

### Sales Configuration
```csharp
private void ConfigureSales(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Sales>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        
        entity.Property(e => e.PersonnelId)
            .IsRequired()
            .HasColumnName("personnel_id");
            
        entity.Property(e => e.ReportDate)
            .IsRequired()
            .HasColumnName("report_date");
            
        entity.Property(e => e.SalesAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasColumnName("sales_amount");
            
        // Foreign key relationship with cascade delete
        entity.HasOne(e => e.Personnel)
            .WithMany(p => p.Sales)
            .HasForeignKey(e => e.PersonnelId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Index for performance
        entity.HasIndex(e => e.PersonnelId);
        entity.HasIndex(e => e.ReportDate);
    });
}
```

### CommissionProfile Configuration
```csharp
private void ConfigureCommissionProfile(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<CommissionProfile>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        
        entity.Property(e => e.ProfileName)
            .IsRequired()
            .HasColumnName("profile_name");
            
        entity.Property(e => e.CommissionFixed)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasColumnName("commission_fixed");
            
        entity.Property(e => e.CommissionPercentage)
            .IsRequired()
            .HasColumnType("decimal(10,6)")
            .HasColumnName("commission_percentage");
    });
}
```

## Repository Pattern

### Generic Repository Interface
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

### Specific Repository Interfaces
```csharp
public interface IPersonnelRepository : IRepository<Personnel>
{
    Task<IEnumerable<Personnel>> GetWithCommissionProfileAsync();
    Task<Personnel> GetWithSalesAsync(int id);
    Task<bool> HasSalesRecordsAsync(int id);
}

public interface ISalesRepository : IRepository<Sales>
{
    Task<IEnumerable<Sales>> GetByPersonnelIdAsync(int personnelId);
    Task<IEnumerable<Sales>> GetByDateRangeAsync(int personnelId, DateTime from, DateTime to);
    Task<decimal> GetMonthlySalesAsync(int personnelId, int month, int year);
}

public interface ICommissionProfileRepository : IRepository<CommissionProfile>
{
    Task<bool> IsReferencedByPersonnelAsync(int id);
}
```

## Repository Implementations

### Personnel Repository
```csharp
public class PersonnelRepository : Repository<Personnel>, IPersonnelRepository
{
    public PersonnelRepository(MarketingDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Personnel>> GetWithCommissionProfileAsync()
    {
        return await _context.Personnel
            .Include(p => p.CommissionProfile)
            .ToListAsync();
    }
    
    public async Task<Personnel> GetWithSalesAsync(int id)
    {
        return await _context.Personnel
            .Include(p => p.Sales)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<bool> HasSalesRecordsAsync(int id)
    {
        return await _context.Sales
            .AnyAsync(s => s.PersonnelId == id);
    }
}
```

### Sales Repository
```csharp
public class SalesRepository : Repository<Sales>, ISalesRepository
{
    public SalesRepository(MarketingDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Sales>> GetByPersonnelIdAsync(int personnelId)
    {
        return await _context.Sales
            .Where(s => s.PersonnelId == personnelId)
            .OrderByDescending(s => s.ReportDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Sales>> GetByDateRangeAsync(int personnelId, DateTime from, DateTime to)
    {
        return await _context.Sales
            .Where(s => s.PersonnelId == personnelId && 
                       s.ReportDate >= from && 
                       s.ReportDate <= to)
            .OrderBy(s => s.ReportDate)
            .ToListAsync();
    }
    
    public async Task<decimal> GetMonthlySalesAsync(int personnelId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        return await _context.Sales
            .Where(s => s.PersonnelId == personnelId && 
                       s.ReportDate >= startDate && 
                       s.ReportDate <= endDate)
            .SumAsync(s => s.SalesAmount);
    }
}
```

## Unit of Work Pattern

### IUnitOfWork Interface
```csharp
public interface IUnitOfWork : IDisposable
{
    IPersonnelRepository Personnel { get; }
    ISalesRepository Sales { get; }
    ICommissionProfileRepository CommissionProfiles { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### UnitOfWork Implementation
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly MarketingDbContext _context;
    private IDbContextTransaction _transaction;
    
    public UnitOfWork(MarketingDbContext context)
    {
        _context = context;
        Personnel = new PersonnelRepository(_context);
        Sales = new SalesRepository(_context);
        CommissionProfiles = new CommissionProfileRepository(_context);
    }
    
    public IPersonnelRepository Personnel { get; }
    public ISalesRepository Sales { get; }
    public ICommissionProfileRepository CommissionProfiles { get; }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        await _transaction?.CommitAsync();
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _transaction?.RollbackAsync();
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}
```

## Migrations

### Initial Migration
```bash
# Add initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Generate SQL script
dotnet ef migrations script --output SQL/create_tables.sql
```

### Migration Commands
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Remove last migration
dotnet ef migrations remove

# Update to specific migration
dotnet ef database update MigrationName

# Generate SQL for production
dotnet ef migrations script FromMigration ToMigration
```

## Seed Data

### Data Seeding
```csharp
public static class DbInitializer
{
    public static async Task SeedAsync(MarketingDbContext context)
    {
        // Seed Commission Profiles
        if (!context.CommissionProfile.Any())
        {
            var profiles = new[]
            {
                new CommissionProfile { ProfileName = 1, CommissionFixed = 500.00m, CommissionPercentage = 0.05m },
                new CommissionProfile { ProfileName = 2, CommissionFixed = 750.00m, CommissionPercentage = 0.07m },
                new CommissionProfile { ProfileName = 3, CommissionFixed = 1000.00m, CommissionPercentage = 0.10m }
            };
            
            context.CommissionProfile.AddRange(profiles);
            await context.SaveChangesAsync();
        }
        
        // Seed Personnel
        if (!context.Personnel.Any())
        {
            var personnel = new[]
            {
                new Personnel { Name = "John Smith", Age = 25, Phone = "123-456-7890", CommissionProfileId = 1, BankName = "ABC Bank", BankAccountNo = "12345" },
                new Personnel { Name = "Jane Doe", Age = 30, Phone = "098-765-4321", CommissionProfileId = 2, BankName = "XYZ Bank", BankAccountNo = "67890" }
            };
            
            context.Personnel.AddRange(personnel);
            await context.SaveChangesAsync();
        }
        
        // Seed Sales
        if (!context.Sales.Any())
        {
            var sales = GenerateSampleSales();
            context.Sales.AddRange(sales);
            await context.SaveChangesAsync();
        }
    }
}
```

## Performance Optimization

### Query Optimization
- Use `Include()` for eager loading
- Apply `AsNoTracking()` for read-only queries
- Implement pagination for large datasets
- Use indexes on frequently queried columns

### Connection Management
- Connection pooling enabled by default
- Proper disposal of contexts
- Transaction scope management

## Error Handling

### Database Exceptions
```csharp
public async Task<T> HandleDbOperationAsync<T>(Func<Task<T>> operation)
{
    try
    {
        return await operation();
    }
    catch (DbUpdateException ex)
    {
        // Handle constraint violations, etc.
        throw new DataAccessException("Database operation failed", ex);
    }
    catch (SqlException ex)
    {
        // Handle SQL Server specific errors
        throw new DatabaseConnectionException("Database connection failed", ex);
    }
}
```

## Configuration

### Dependency Injection
```csharp
services.AddDbContext<MarketingDbContext>(options =>
    options.UseSqlServer(connectionString));
    
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IPersonnelRepository, PersonnelRepository>();
services.AddScoped<ISalesRepository, SalesRepository>();
services.AddScoped<ICommissionProfileRepository, CommissionProfileRepository>();
```

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Marketing;User Id=sa;Password=999999;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

## Dependencies

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
```
