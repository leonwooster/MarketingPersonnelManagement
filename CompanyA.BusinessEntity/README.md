# CompanyA.BusinessEntity

Data Transfer Objects (DTOs) and business entities for the Marketing Personnel Management System.

## Overview

This project contains the data contracts and business entities used throughout the application. It defines the structure for data transfer between layers and external API communication.

## Technology Stack

- **Framework**: .NET 9.0 Class Library
- **Serialization**: System.Text.Json compatible
- **Validation**: Data Annotations

## Data Transfer Objects (DTOs)

### PersonnelDto
Represents personnel information for API communication.

```csharp
public class PersonnelDto
{
    public int Id { get; set; }
    
    [Required, StringLength(50)]
    public string Name { get; set; }
    
    [Required, Range(19, int.MaxValue)]
    public int Age { get; set; }
    
    [Required, StringLength(20)]
    public string Phone { get; set; }
    
    [Required]
    public int CommissionProfileId { get; set; }
    
    [StringLength(20)]
    public string? BankName { get; set; }
    
    [StringLength(20)]
    public string? BankAccountNo { get; set; }
    
    // Navigation property
    public CommissionProfileDto? CommissionProfile { get; set; }
    
    // Future authentication property
    public int? ManagerId { get; set; }
    public ManagerDto? Manager { get; set; }
}
```

**Validation Rules**:
- Name: Required, 1-50 characters
- Age: Required, minimum 19
- Phone: Required, 1-20 characters
- CommissionProfileId: Required, must reference existing profile
- BankName: Optional, max 20 characters
- BankAccountNo: Optional, max 20 characters

### SalesDto
Represents sales record information.

```csharp
public class SalesDto
{
    public int Id { get; set; }
    
    [Required]
    public int PersonnelId { get; set; }
    
    [Required]
    public DateTime ReportDate { get; set; }
    
    [Required, Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal SalesAmount { get; set; }
    
    // Navigation property
    public PersonnelDto? Personnel { get; set; }
}
```

**Validation Rules**:
- PersonnelId: Required, must reference existing personnel
- ReportDate: Required, cannot be future date
- SalesAmount: Required, >= 0, decimal(10,2) precision

### CommissionProfileDto
Represents commission profile configuration.

```csharp
public class CommissionProfileDto
{
    public int Id { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int ProfileName { get; set; }
    
    [Required, Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal CommissionFixed { get; set; }
    
    [Required, Range(0, 1)]
    [Column(TypeName = "decimal(10,6)")]
    public decimal CommissionPercentage { get; set; }
    
    // Navigation property
    public List<PersonnelDto>? Personnel { get; set; }
}
```

**Validation Rules**:
- ProfileName: Required, positive integer
- CommissionFixed: Required, >= 0, decimal(10,2) precision
- CommissionPercentage: Required, 0-1 range, decimal(10,6) precision

### ManagerDto
Represents manager information for authentication system (Sprint 4).

```csharp
public class ManagerDto
{
    public int Id { get; set; }
    
    [Required, StringLength(50)]
    public string Username { get; set; }
    
    [Required, StringLength(100)]
    public string FullName { get; set; }
    
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; }
    
    // Password not included in DTO for security
    // Navigation property
    public List<PersonnelDto>? Personnel { get; set; }
}
```

**Validation Rules**:
- Username: Required, unique, 1-50 characters
- FullName: Required, 1-100 characters
- Email: Required, valid email format, 1-100 characters

## Report DTOs

### ManagementOverviewDto
Management dashboard report data.

```csharp
public class ManagementOverviewDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AverageSalesPerPerson { get; set; }
    public int DaysWithNoSales { get; set; }
    public List<TopPerformerDto> TopPerformers { get; set; }
    public List<MonthlySalesDto> MonthlySales { get; set; }
}

public class TopPerformerDto
{
    public string PersonnelName { get; set; }
    public decimal TotalSales { get; set; }
    public int Rank { get; set; }
}

public class MonthlySalesDto
{
    public int Day { get; set; }
    public decimal SalesAmount { get; set; }
}
```

### CommissionPayoutDto
Finance commission calculation report.

```csharp
public class CommissionPayoutDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<PersonnelCommissionDto> PersonnelCommissions { get; set; }
    public decimal TotalCommissionPayout { get; set; }
}

public class PersonnelCommissionDto
{
    public string PersonnelName { get; set; }
    public decimal MonthlySales { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionVariable { get; set; }
    public decimal TotalCommission { get; set; }
}
```

## Request/Response Models

### API Request Models
```csharp
public class CreatePersonnelRequest
{
    [Required, StringLength(50)]
    public string Name { get; set; }
    
    [Required, Range(19, int.MaxValue)]
    public int Age { get; set; }
    
    [Required, StringLength(20)]
    public string Phone { get; set; }
    
    [Required]
    public int CommissionProfileId { get; set; }
    
    [StringLength(20)]
    public string? BankName { get; set; }
    
    [StringLength(20)]
    public string? BankAccountNo { get; set; }
}

public class CreateSalesRequest
{
    [Required]
    public int PersonnelId { get; set; }
    
    [Required]
    public DateTime ReportDate { get; set; }
    
    [Required, Range(0, double.MaxValue)]
    public decimal SalesAmount { get; set; }
}

public class LoginRequest
{
    [Required, StringLength(50)]
    public string Username { get; set; }
    
    [Required, StringLength(100)]
    public string Password { get; set; }
}
```

### API Response Models
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
}

public class ApiError
{
    public string Code { get; set; }
    public string Message { get; set; }
    public List<ValidationError>? Details { get; set; }
}

public class ValidationError
{
    public string Field { get; set; }
    public string Message { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

## Validation Attributes

### Custom Validation
```csharp
public class NotFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date.Date <= DateTime.Today;
        }
        return true;
    }
}

public class PositiveIntegerAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is int intValue)
        {
            return intValue > 0;
        }
        return false;
    }
}
```

## JSON Serialization

### Configuration
DTOs are configured for System.Text.Json with:
- Camel case property naming
- Null value handling
- Date format standardization
- Enum string conversion

### Example JSON
```json
{
  "id": 1,
  "name": "John Doe",
  "age": 25,
  "phone": "123-456-7890",
  "commissionProfileId": 1,
  "bankName": "ABC Bank",
  "bankAccountNo": "12345678",
  "commissionProfile": {
    "id": 1,
    "profileName": 100,
    "commissionFixed": 500.00,
    "commissionPercentage": 0.05
  }
}
```

## Business Rules Enforcement

### Data Integrity
- Foreign key relationships maintained
- Required field validation
- Data type and range validation
- String length limitations

### Business Logic
- Age minimum enforcement (19 years)
- Sales amount non-negative validation
- Date validation (no future sales)
- Commission calculation precision

## Mapping Patterns

### Entity to DTO Mapping
```csharp
public static PersonnelDto ToDto(this Personnel entity)
{
    return new PersonnelDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Age = entity.Age,
        Phone = entity.Phone,
        CommissionProfileId = entity.CommissionProfileId,
        BankName = entity.BankName,
        BankAccountNo = entity.BankAccountNo,
        CommissionProfile = entity.CommissionProfile?.ToDto()
    };
}
```

## Future Enhancements

### Authentication DTOs
- Login/logout request/response models
- JWT token models
- User session information
- Role-based access control models

### Advanced Features
- Audit trail DTOs
- Bulk operation models
- Import/export formats
- Notification models

## Dependencies

This project has no external dependencies and serves as the contract layer for the entire application.

## Usage Guidelines

### API Controllers
- Use request models for input validation
- Return response models with consistent structure
- Include navigation properties when needed
- Handle null values appropriately

### Business Services
- Map between entities and DTOs
- Validate business rules at DTO level
- Use DTOs for inter-service communication
- Maintain data consistency

### Client Applications
- Use DTOs for API communication
- Implement client-side validation
- Handle error responses gracefully
- Cache frequently used reference data