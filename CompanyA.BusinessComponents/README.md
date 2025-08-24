# CompanyA.BusinessComponents

Business logic layer for the Marketing Personnel Management System.

## Overview

This class library contains the core business logic, validation rules, and domain services for managing marketing personnel, sales records, and commission calculations.

## Architecture

### Service Layer Pattern
- **PersonnelService**: Personnel management operations
- **SalesService**: Sales record operations  
- **CommissionService**: Commission calculations and profiles
- **ReportService**: Business reporting and analytics

### Dependency Injection
All services are registered with DI container and injected into API controllers.

## Services

### PersonnelService
```csharp
public interface IPersonnelService
{
    Task<IEnumerable<PersonnelDto>> GetAllAsync();
    Task<PersonnelDto> GetByIdAsync(int id);
    Task<PersonnelDto> CreateAsync(CreatePersonnelDto dto);
    Task<PersonnelDto> UpdateAsync(int id, UpdatePersonnelDto dto);
    Task<bool> DeleteAsync(int id, bool confirmed);
    Task<bool> ExistsAsync(int id);
}
```

### SalesService
```csharp
public interface ISalesService
{
    Task<IEnumerable<SalesDto>> GetByPersonnelAsync(int personnelId, DateTime? from, DateTime? to);
    Task<SalesDto> CreateAsync(CreateSalesDto dto);
    Task<bool> DeleteAsync(int id);
    Task<decimal> GetMonthlySalesAsync(int personnelId, int month, int year);
}
```

### CommissionService
```csharp
public interface ICommissionService
{
    Task<IEnumerable<CommissionProfileDto>> GetAllProfilesAsync();
    Task<CommissionProfileDto> GetProfileByIdAsync(int id);
    Task<CommissionProfileDto> CreateProfileAsync(CreateCommissionProfileDto dto);
    Task<CommissionProfileDto> UpdateProfileAsync(int id, UpdateCommissionProfileDto dto);
    Task<bool> DeleteProfileAsync(int id);
    Task<decimal> CalculateMonthlyCommissionAsync(int personnelId, int month, int year);
}
```

### ReportService
```csharp
public interface IReportService
{
    Task<ManagementReportDto> GetManagementReportAsync(int month, int year);
    Task<CommissionReportDto> GetCommissionReportAsync(int month, int year);
    Task<byte[]> ExportReportAsync(string reportType, string format, int month, int year);
}
```

## Business Rules

### Personnel Management
1. **Name Validation**: Required, 1-50 characters, trimmed
2. **Age Validation**: Must be >= 19 years old
3. **Phone Validation**: Required, 1-20 characters, non-empty
4. **Commission Profile**: Must reference existing profile
5. **Deletion Rules**: Requires confirmation, cascades to sales

### Sales Management
1. **Personnel Reference**: Must exist in Personnel table
2. **Date Validation**: Cannot be future date
3. **Amount Validation**: Must be >= 0, decimal(10,2) precision
4. **Edit Restriction**: No edit operations allowed, only add/delete
5. **Cascade Delete**: Removed when personnel is deleted

### Commission Calculations
1. **Formula**: `Commission = Fixed + (Percentage × Monthly Sales)`
2. **Precision**: Fixed as decimal(10,2), Percentage as decimal(10,6)
3. **Validation**: Both values must be >= 0
4. **Profile Deletion**: Blocked if referenced by personnel

## Validation Framework

### Custom Validators
```csharp
public class PersonnelValidator : AbstractValidator<CreatePersonnelDto>
{
    public PersonnelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
            
        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(19).WithMessage("Age must be 19 or older");
            
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters");
    }
}
```

### Business Rule Validation
```csharp
public async Task<ValidationResult> ValidatePersonnelDeletionAsync(int id)
{
    var result = new ValidationResult();
    
    var personnel = await _repository.GetByIdAsync(id);
    if (personnel == null)
    {
        result.AddError("Personnel not found");
        return result;
    }
    
    // Additional business rules can be added here
    return result;
}
```

## Domain Models

### Personnel Domain
```csharp
public class Personnel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }
    public int CommissionProfileId { get; set; }
    public string BankName { get; set; }
    public string BankAccountNo { get; set; }
    
    // Navigation properties
    public CommissionProfile CommissionProfile { get; set; }
    public ICollection<Sales> Sales { get; set; }
}
```

### Sales Domain
```csharp
public class Sales
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }
    public DateTime ReportDate { get; set; }
    public decimal SalesAmount { get; set; }
    
    // Navigation properties
    public Personnel Personnel { get; set; }
}
```

### Commission Profile Domain
```csharp
public class CommissionProfile
{
    public int Id { get; set; }
    public int ProfileName { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
    
    // Navigation properties
    public ICollection<Personnel> Personnel { get; set; }
}
```

## Exception Handling

### Custom Exceptions
```csharp
public class BusinessValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }
    
    public BusinessValidationException(IEnumerable<ValidationError> errors)
        : base("One or more business validation errors occurred")
    {
        Errors = errors;
    }
}

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityType, int id)
        : base($"{entityType} with ID {id} not found")
    {
    }
}
```

## Reporting Logic

### Management Reports
```csharp
public async Task<ManagementReportDto> GetManagementReportAsync(int month, int year)
{
    var startDate = new DateTime(year, month, 1);
    var endDate = startDate.AddMonths(1).AddDays(-1);
    
    return new ManagementReportDto
    {
        TotalSales = await CalculateTotalSalesAsync(startDate, endDate),
        TopPerformers = await GetTopPerformersAsync(startDate, endDate, 5),
        AveragePerPerson = await CalculateAveragePerPersonAsync(startDate, endDate),
        DaysWithoutSales = await CountDaysWithoutSalesAsync(startDate, endDate)
    };
}
```

### Commission Reports
```csharp
public async Task<CommissionReportDto> GetCommissionReportAsync(int month, int year)
{
    var personnel = await _personnelRepository.GetAllAsync();
    var commissions = new List<PersonnelCommissionDto>();
    
    foreach (var person in personnel)
    {
        var monthlySales = await GetMonthlySalesAsync(person.Id, month, year);
        var commission = await CalculateCommissionAsync(person, monthlySales);
        
        commissions.Add(new PersonnelCommissionDto
        {
            PersonnelId = person.Id,
            PersonnelName = person.Name,
            MonthlySales = monthlySales,
            CommissionAmount = commission
        });
    }
    
    return new CommissionReportDto
    {
        Month = month,
        Year = year,
        PersonnelCommissions = commissions,
        TotalCommissions = commissions.Sum(c => c.CommissionAmount)
    };
}
```

## Performance Considerations

### Caching Strategy
- Cache commission profiles (rarely change)
- Cache monthly aggregations
- Use memory cache for frequently accessed data

### Async Operations
- All database operations are async
- Proper cancellation token usage
- Efficient LINQ queries

## Testing

### Unit Testing
```csharp
[Test]
public async Task CalculateMonthlyCommission_ValidData_ReturnsCorrectAmount()
{
    // Arrange
    var personnelId = 1;
    var month = 10;
    var year = 2023;
    var expectedCommission = 1500.50m;
    
    // Act
    var result = await _commissionService.CalculateMonthlyCommissionAsync(personnelId, month, year);
    
    // Assert
    Assert.AreEqual(expectedCommission, result);
}
```

## Dependencies

```xml
<PackageReference Include="FluentValidation" />
<PackageReference Include="AutoMapper" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" />
<PackageReference Include="Microsoft.Extensions.Logging" />
```

## Configuration

### Service Registration
```csharp
services.AddScoped<IPersonnelService, PersonnelService>();
services.AddScoped<ISalesService, SalesService>();
services.AddScoped<ICommissionService, CommissionService>();
services.AddScoped<IReportService, ReportService>();
```
