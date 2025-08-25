# CompanyA.DataAccess

Data access layer for the Marketing Personnel Management System using Entity Framework Core.

## Overview

This project provides data access functionality through Entity Framework Core, including database context, entity models, migrations, and repository patterns. It handles all database operations and maintains data integrity.

## Technology Stack

- **Framework**: .NET 9.0 Class Library
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server 2016+
- **Migrations**: EF Core Code-First approach

## Database Context

### MarketingDbContext
Main database context managing all entities and relationships.

```csharp
public class MarketingDbContext : DbContext
{
    public DbSet<Personnel> Personnel { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<CommissionProfile> CommissionProfiles { get; set; }
    public DbSet<Manager> Managers { get; set; } // Sprint 4
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
        // Seed data
        // Relationships
    }
}
```

## Entity Models

### Personnel
Represents marketing personnel information.

```csharp
public class Personnel
{
    public int Id { get; set; }
    
    [Required, StringLength(50)]
    public string Name { get; set; }
    
    [Required, Range(19, int.MaxValue)]
    public int Age { get; set; }
    
    [Required, StringLength(20)]
    public string Phone { get; set; }
    
    [StringLength(20)]
    public string? BankName { get; set; }
    
    [StringLength(20)]
    public string? BankAccountNo { get; set; }
    
    // Foreign Keys
    public int CommissionProfileId { get; set; }
    public int? ManagerId { get; set; } // Sprint 4
    
    // Navigation Properties
    public CommissionProfile CommissionProfile { get; set; }
    public Manager? Manager { get; set; } // Sprint 4
    public ICollection<Sales> Sales { get; set; } = new List<Sales>();
}
```

### Sales
Represents sales records for personnel.

```csharp
public class Sales
{
    public int Id { get; set; }
    
    [Required]
    public int PersonnelId { get; set; }
    
    [Required]
    public DateTime ReportDate { get; set; }
    
    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal SalesAmount { get; set; }
    
    // Navigation Properties
    public Personnel Personnel { get; set; }
}
```

### CommissionProfile
Represents commission calculation profiles.

```csharp
public class CommissionProfile
{
    public int Id { get; set; }
    
    [Required]
    public int ProfileName { get; set; }
    
    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal CommissionFixed { get; set; }
    
    [Required, Column(TypeName = "decimal(10,6)")]
    public decimal CommissionPercentage { get; set; }
    
    // Navigation Properties
    public ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
}
```

### Manager (Sprint 4)
Represents managers for authentication and data isolation.

```csharp
public class Manager
{
    public int Id { get; set; }
    
    [Required, StringLength(50)]
    public string Username { get; set; }
    
    [Required, StringLength(255)]
    public string PasswordHash { get; set; }
    
    [Required, StringLength(100)]
    public string FullName { get; set; }
    
    [Required, StringLength(100)]
    public string Email { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
}
```

## Entity Configurations

### Fluent API Configuration
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Personnel Configuration
    modelBuilder.Entity<Personnel>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
        entity.Property(e => e.BankName).HasMaxLength(20);
        entity.Property(e => e.BankAccountNo).HasMaxLength(20);
        
        entity.HasOne(e => e.CommissionProfile)
              .WithMany(cp => cp.Personnel)
              .HasForeignKey(e => e.CommissionProfileId)
              .OnDelete(DeleteBehavior.Restrict);
              
        entity.HasOne(e => e.Manager)
              .WithMany(m => m.Personnel)
              .HasForeignKey(e => e.ManagerId)
              .OnDelete(DeleteBehavior.Restrict);
    });
    
    // Sales Configuration
    modelBuilder.Entity<Sales>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.SalesAmount).HasColumnType("decimal(10,2)");
        
        entity.HasOne(e => e.Personnel)
              .WithMany(p => p.Sales)
              .HasForeignKey(e => e.PersonnelId)
              .OnDelete(DeleteBehavior.Cascade);
    });
    
    // CommissionProfile Configuration
    modelBuilder.Entity<CommissionProfile>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CommissionFixed).HasColumnType("decimal(10,2)");
        entity.Property(e => e.CommissionPercentage).HasColumnType("decimal(10,6)");
        entity.HasIndex(e => e.ProfileName).IsUnique();
    });
    
    // Manager Configuration
    modelBuilder.Entity<Manager>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
        entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
        entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        entity.HasIndex(e => e.Username).IsUnique();
        entity.HasIndex(e => e.Email).IsUnique();
    });
}
```

## Database Relationships

### Entity Relationship Diagram
```
Manager (1) -----> (0..*) Personnel
Personnel (1) -----> (0..*) Sales
CommissionProfile (1) -----> (0..*) Personnel
```

### Foreign Key Constraints
- **Personnel.CommissionProfileId** → CommissionProfile.Id (Restrict)
- **Personnel.ManagerId** → Manager.Id (Restrict)
- **Sales.PersonnelId** → Personnel.Id (Cascade)

## Migrations

### Current Migrations
1. **Initial**: Creates base tables (Personnel, Sales, CommissionProfile)
2. **SeedData**: Adds initial commission profiles and test data
3. **UpdateSeedDataTo2025**: Updates test data for current year
4. **AddManagerAuthentication**: Adds Manager table and foreign key (Sprint 4)

### Migration Commands
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Generate SQL script
dotnet ef migrations script

# Remove last migration (if not applied)
dotnet ef migrations remove
```

## Seed Data

### Commission Profiles
```csharp
modelBuilder.Entity<CommissionProfile>().HasData(
    new CommissionProfile { Id = 1, ProfileName = 100, CommissionFixed = 500m, CommissionPercentage = 0.05m },
    new CommissionProfile { Id = 2, ProfileName = 200, CommissionFixed = 750m, CommissionPercentage = 0.075m },
    new CommissionProfile { Id = 3, ProfileName = 300, CommissionFixed = 1000m, CommissionPercentage = 0.10m }
);
```

### Test Personnel
```csharp
modelBuilder.Entity<Personnel>().HasData(
    new Personnel { Id = 1, Name = "John Doe", Age = 25, Phone = "123-456-7890", CommissionProfileId = 1 },
    new Personnel { Id = 2, Name = "Jane Smith", Age = 30, Phone = "098-765-4321", CommissionProfileId = 2 },
    new Personnel { Id = 3, Name = "Bob Johnson", Age = 28, Phone = "555-123-4567", CommissionProfileId = 1 }
);
```

### Test Managers (Sprint 4)
```csharp
modelBuilder.Entity<Manager>().HasData(
    new Manager 
    { 
        Id = 1, 
        Username = "manager1", 
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        FullName = "Manager One",
        Email = "manager1@company.com",
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    }
);
```

## Connection String Configuration

### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Marketing;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### Production
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PRODUCTION_SERVER;Database=Marketing;User Id=marketing_user;Password=SECURE_PASSWORD;TrustServerCertificate=True"
  }
}
```

## Query Patterns

### Common Queries
```csharp
// Get personnel with commission profile
var personnel = await context.Personnel
    .Include(p => p.CommissionProfile)
    .Where(p => p.ManagerId == managerId) // Sprint 4 filtering
    .ToListAsync();

// Get sales with personnel info
var sales = await context.Sales
    .Include(s => s.Personnel)
    .Where(s => s.ReportDate >= startDate && s.ReportDate <= endDate)
    .ToListAsync();

// Get monthly sales totals
var monthlySales = await context.Sales
    .Where(s => s.ReportDate.Month == month && s.ReportDate.Year == year)
    .GroupBy(s => s.PersonnelId)
    .Select(g => new { PersonnelId = g.Key, Total = g.Sum(s => s.SalesAmount) })
    .ToListAsync();
```

### Performance Optimization
- Use `AsNoTracking()` for read-only queries
- Include related data efficiently with `Include()`
- Use pagination for large result sets
- Index frequently queried columns

## Data Validation

### Database Constraints
- Primary keys on all entities
- Foreign key constraints with appropriate cascade behavior
- Unique constraints on business keys (Username, ProfileName)
- Check constraints for business rules (Age >= 19, SalesAmount >= 0)

### Entity Validation
- Data annotations on entity properties
- Custom validation attributes
- Business rule validation in DbContext
- Concurrency conflict handling

## Transaction Management

### Unit of Work Pattern
```csharp
using var transaction = await context.Database.BeginTransactionAsync();
try
{
    // Multiple operations
    await context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

## Logging and Monitoring

### EF Core Logging
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
                  .EnableSensitiveDataLogging() // Development only
                  .EnableDetailedErrors();
}
```

### Query Performance
- Log slow queries
- Monitor connection pool usage
- Track database operation metrics
- Analyze query execution plans

## Security Considerations

### Data Protection
- Sensitive data encryption at rest
- Connection string security
- SQL injection prevention (parameterized queries)
- Access control through application layer

### Authentication Integration
- Manager-based data filtering
- Row-level security implementation
- Audit trail for data changes
- Secure password storage

## Testing

### Unit Testing
```csharp
[Test]
public async Task CanCreatePersonnel()
{
    using var context = CreateInMemoryContext();
    var personnel = new Personnel { Name = "Test", Age = 25, Phone = "123" };
    
    context.Personnel.Add(personnel);
    await context.SaveChangesAsync();
    
    Assert.That(personnel.Id, Is.GreaterThan(0));
}
```

### Integration Testing
- Database integration tests
- Migration testing
- Performance testing with realistic data
- Concurrency testing

## Deployment

### Database Setup
1. Create SQL Server database
2. Configure connection string
3. Run migrations: `dotnet ef database update`
4. Verify seed data

### Production Considerations
- Connection pooling configuration
- Backup and recovery procedures
- Performance monitoring
- Index optimization

## Dependencies

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
<PackageReference Include="BCrypt.Net-Next" /> <!-- Sprint 4 -->
```

## Future Enhancements

### Audit Trail
- Track all data changes
- User attribution for changes
- Timestamp tracking
- Change history queries

### Advanced Features
- Soft delete implementation
- Multi-tenancy support
- Database sharding
- Read replicas for reporting