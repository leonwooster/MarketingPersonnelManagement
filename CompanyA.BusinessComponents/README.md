# CompanyA.BusinessComponents

Business logic layer for the Marketing Personnel Management System.

## Overview

This project contains the business services and logic that orchestrate operations between the API controllers and data access layer. It implements business rules, validation, and complex operations that span multiple entities.

## Technology Stack

- **Framework**: .NET 9.0 Class Library
- **Dependencies**: CompanyA.DataAccess, CompanyA.BusinessEntity
- **Patterns**: Service Layer, Dependency Injection

## Services

### IPersonnelService / PersonnelService
Manages personnel business operations:
- **GetAllPersonnelAsync()**: Retrieves all personnel with commission profiles
- **GetPersonnelByIdAsync(id)**: Gets single personnel by ID
- **CreatePersonnelAsync(dto)**: Creates new personnel with validation
- **UpdatePersonnelAsync(id, dto)**: Updates existing personnel
- **DeletePersonnelAsync(id, confirm)**: Soft/hard delete with confirmation

**Business Rules**:
- Age must be >= 19
- Name is required and limited to 50 characters
- Phone is required and limited to 20 characters
- Commission profile must exist
- Bank details are optional but limited to 20 characters each

### ISalesService / SalesService
Manages sales record operations:
- **GetSalesByPersonnelAsync(personnelId, from, to)**: Filtered sales retrieval
- **CreateSalesAsync(dto)**: Adds new sales record
- **DeleteSalesAsync(id)**: Removes sales record

**Business Rules**:
- Sales amount must be >= 0
- Report date cannot be in the future
- Personnel must exist for foreign key
- No editing allowed - add/delete only

### ICommissionProfileService / CommissionProfileService
Manages commission profile operations:
- **GetAllProfilesAsync()**: Lists all commission profiles
- **GetProfileByIdAsync(id)**: Gets single profile
- **CreateProfileAsync(dto)**: Creates new profile
- **UpdateProfileAsync(id, dto)**: Updates existing profile
- **DeleteProfileAsync(id)**: Deletes if not referenced

**Business Rules**:
- Profile name must be positive integer
- Commission fixed must be >= 0
- Commission percentage must be >= 0
- Cannot delete if personnel reference exists

### IReportsService / ReportsService
Generates business reports:
- **GetManagementOverviewAsync(month, year)**: Management dashboard data
- **GetCommissionPayoutAsync(month, year)**: Finance commission calculations
- **ExportReportAsync(type, format)**: CSV/JSON export functionality

**Report Types**:
- **Management Overview**: Monthly totals, top performers, averages, no-sales days
- **Commission Payout**: Per-person sales and commission calculations

## Data Transfer Objects (DTOs)

### PersonnelDto
```csharp
public class PersonnelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }
    public int CommissionProfileId { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public CommissionProfileDto? CommissionProfile { get; set; }
}
```

### SalesDto
```csharp
public class SalesDto
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }
    public DateTime ReportDate { get; set; }
    public decimal SalesAmount { get; set; }
    public PersonnelDto? Personnel { get; set; }
}
```

### CommissionProfileDto
```csharp
public class CommissionProfileDto
{
    public int Id { get; set; }
    public int ProfileName { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
}
```

## Validation

### Input Validation
- Data annotations on DTOs
- Custom validation attributes where needed
- Business rule validation in service methods
- Async validation for database constraints

### Error Handling
- Custom business exceptions
- Validation result patterns
- Consistent error messaging
- Logging of business rule violations

## Dependency Injection

Services are registered in the API's Program.cs:
```csharp
builder.Services.AddScoped<IPersonnelService, PersonnelService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<ICommissionProfileService, CommissionProfileService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
```

## Business Rules Implementation

### Personnel Management
1. **Age Validation**: Minimum 19 years enforced at service level
2. **Unique Constraints**: Name uniqueness checked before creation
3. **Cascade Deletion**: Personnel deletion removes associated sales
4. **Commission Profile**: Must exist before personnel assignment

### Sales Management
1. **Date Validation**: Report date cannot be future date
2. **Amount Validation**: Sales amount must be non-negative
3. **Immutability**: Sales records cannot be edited once created
4. **Personnel Reference**: Must reference existing personnel

### Commission Calculation
1. **Formula**: `commission_fixed + (commission_percentage × monthly_sales)`
2. **Precision**: Decimal calculations with proper rounding
3. **Monthly Aggregation**: Sales grouped by month for calculation
4. **Zero Handling**: Proper handling of zero sales months

## Testing

### Unit Testing
- Service method testing with mocked dependencies
- Business rule validation testing
- Edge case handling verification
- Async operation testing

### Integration Testing
- Database integration testing
- Cross-service operation testing
- Transaction boundary testing
- Performance testing for large datasets

## Performance Considerations

### Async Operations
- All database operations are async
- Proper async/await usage throughout
- Cancellation token support where applicable

### Caching Strategy
- Consider implementing caching for:
  - Commission profiles (rarely change)
  - Personnel lookup data
  - Report calculations (time-based cache)

### Query Optimization
- Efficient LINQ queries
- Proper use of Include() for related data
- Pagination for large result sets
- Indexed database queries

## Future Enhancements

### Authentication Integration
- Manager-based data filtering
- Role-based authorization
- User context injection
- Audit trail implementation

### Advanced Features
- Bulk operations support
- Data import/export functionality
- Advanced reporting with charts
- Email notification system

## Dependencies

```xml
<ProjectReference Include="..\CompanyA.DataAccess\CompanyA.DataAccess.csproj" />
<ProjectReference Include="..\CompanyA.BusinessEntity\CompanyA.BusinessEntity.csproj" />
```

## Development Guidelines

### Code Standards
- Follow SOLID principles
- Implement proper separation of concerns
- Use dependency injection consistently
- Write comprehensive unit tests

### Error Handling
- Use specific exception types
- Provide meaningful error messages
- Log business rule violations
- Return appropriate HTTP status codes

### Documentation
- Document all public methods
- Include business rule explanations
- Provide usage examples
- Maintain API documentation