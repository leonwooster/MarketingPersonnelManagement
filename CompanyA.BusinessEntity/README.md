# CompanyA.BusinessEntity

Domain models and Data Transfer Objects (DTOs) for the Marketing Personnel Management System.

## Overview

This class library contains all domain entities, DTOs, and value objects used throughout the application. Provides a clean separation between database models and API contracts.

## Domain Entities

### Personnel Entity
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
    
    // Navigation Properties
    public virtual CommissionProfile CommissionProfile { get; set; }
    public virtual ICollection<Sales> Sales { get; set; } = new List<Sales>();
}
```

### Sales Entity
```csharp
public class Sales
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }
    public DateTime ReportDate { get; set; }
    public decimal SalesAmount { get; set; }
    
    // Navigation Properties
    public virtual Personnel Personnel { get; set; }
}
```

### CommissionProfile Entity
```csharp
public class CommissionProfile
{
    public int Id { get; set; }
    public int ProfileName { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
    
    // Navigation Properties
    public virtual ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
}
```

## Data Transfer Objects (DTOs)

### Personnel DTOs
```csharp
public class PersonnelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }
    public int CommissionProfileId { get; set; }
    public string BankName { get; set; }
    public string BankAccountNo { get; set; }
    public string CommissionProfileName { get; set; }
}

public class CreatePersonnelDto
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }
    public int CommissionProfileId { get; set; }
    public string BankName { get; set; }
    public string BankAccountNo { get; set; }
}

public class UpdatePersonnelDto
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }
    public int CommissionProfileId { get; set; }
    public string BankName { get; set; }
    public string BankAccountNo { get; set; }
}
```

### Sales DTOs
```csharp
public class SalesDto
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }
    public DateTime ReportDate { get; set; }
    public decimal SalesAmount { get; set; }
    public string PersonnelName { get; set; }
}

public class CreateSalesDto
{
    public int PersonnelId { get; set; }
    public DateTime ReportDate { get; set; }
    public decimal SalesAmount { get; set; }
}

public class MonthlySalesDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int PersonnelId { get; set; }
    public string PersonnelName { get; set; }
    public decimal TotalSales { get; set; }
    public IEnumerable<DailySalesDto> DailySales { get; set; }
}

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}
```

### Commission Profile DTOs
```csharp
public class CommissionProfileDto
{
    public int Id { get; set; }
    public int ProfileName { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
    public int PersonnelCount { get; set; }
}

public class CreateCommissionProfileDto
{
    public int ProfileName { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
}

public class UpdateCommissionProfileDto
{
    public int ProfileName { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
}
```

## Report DTOs

### Management Report DTOs
```csharp
public class ManagementReportDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AveragePerPerson { get; set; }
    public int DaysWithoutSales { get; set; }
    public IEnumerable<TopPerformerDto> TopPerformers { get; set; }
    public IEnumerable<MonthlySummaryDto> MonthlySummary { get; set; }
}

public class TopPerformerDto
{
    public int PersonnelId { get; set; }
    public string PersonnelName { get; set; }
    public decimal TotalSales { get; set; }
    public int Rank { get; set; }
}

public class MonthlySummaryDto
{
    public int Day { get; set; }
    public decimal TotalSales { get; set; }
    public int ActivePersonnel { get; set; }
}
```

### Commission Report DTOs
```csharp
public class CommissionReportDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalCommissions { get; set; }
    public decimal TotalSales { get; set; }
    public IEnumerable<PersonnelCommissionDto> PersonnelCommissions { get; set; }
}

public class PersonnelCommissionDto
{
    public int PersonnelId { get; set; }
    public string PersonnelName { get; set; }
    public decimal MonthlySales { get; set; }
    public decimal CommissionFixed { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionAmount { get; set; }
    public string BankName { get; set; }
    public string BankAccountNo { get; set; }
}
```

## Validation Attributes

### Custom Validation Attributes
```csharp
public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;
    
    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }
    
    public override bool IsValid(object value)
    {
        if (value is int age)
        {
            return age >= _minimumAge;
        }
        return false;
    }
    
    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be {_minimumAge} or older.";
    }
}

public class NotFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
        {
            return date.Date <= DateTime.Today;
        }
        return true; // Allow null values
    }
    
    public override string FormatErrorMessage(string name)
    {
        return $"{name} cannot be a future date.";
    }
}
```

## Value Objects

### Money Value Object
```csharp
public class Money
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
            
        Amount = amount;
        Currency = currency ?? "USD";
    }
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
            
        return new Money(left.Amount + right.Amount, left.Currency);
    }
    
    public override string ToString()
    {
        return $"{Amount:C} {Currency}";
    }
}
```

### DateRange Value Object
```csharp
public class DateRange
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    
    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date");
            
        StartDate = startDate;
        EndDate = endDate;
    }
    
    public bool Contains(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }
    
    public int DaysCount => (EndDate - StartDate).Days + 1;
    
    public static DateRange ForMonth(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return new DateRange(startDate, endDate);
    }
}
```

## Enumerations

### Commission Profile Types
```csharp
public enum CommissionProfileType
{
    Junior = 1,
    Senior = 2,
    Manager = 3
}
```

### Report Types
```csharp
public enum ReportType
{
    Management,
    Commission,
    Sales,
    Performance
}

public enum ExportFormat
{
    Json,
    Csv,
    Excel,
    Pdf
}
```

## Mapping Profiles (AutoMapper)

### Personnel Mapping
```csharp
public class PersonnelMappingProfile : Profile
{
    public PersonnelMappingProfile()
    {
        CreateMap<Personnel, PersonnelDto>()
            .ForMember(dest => dest.CommissionProfileName, 
                      opt => opt.MapFrom(src => src.CommissionProfile.ProfileName.ToString()));
                      
        CreateMap<CreatePersonnelDto, Personnel>();
        CreateMap<UpdatePersonnelDto, Personnel>();
    }
}
```

### Sales Mapping
```csharp
public class SalesMappingProfile : Profile
{
    public SalesMappingProfile()
    {
        CreateMap<Sales, SalesDto>()
            .ForMember(dest => dest.PersonnelName, 
                      opt => opt.MapFrom(src => src.Personnel.Name));
                      
        CreateMap<CreateSalesDto, Sales>();
    }
}
```

## Constants

### Business Constants
```csharp
public static class BusinessConstants
{
    public const int MinimumAge = 19;
    public const int MaxNameLength = 50;
    public const int MaxPhoneLength = 20;
    public const int MaxBankNameLength = 20;
    public const int MaxBankAccountLength = 20;
    
    public const string DefaultCurrency = "USD";
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
}
```

### Database Constants
```csharp
public static class DatabaseConstants
{
    public const string PersonnelTableName = "Personnel";
    public const string SalesTableName = "Sales";
    public const string CommissionProfileTableName = "CommissionProfile";
    
    public const string SalesAmountColumnType = "decimal(10,2)";
    public const string CommissionFixedColumnType = "decimal(10,2)";
    public const string CommissionPercentageColumnType = "decimal(10,6)";
}
```

## Extension Methods

### Entity Extensions
```csharp
public static class PersonnelExtensions
{
    public static bool IsEligibleForCommission(this Personnel personnel)
    {
        return personnel.Age >= BusinessConstants.MinimumAge && 
               personnel.CommissionProfileId > 0;
    }
    
    public static string GetFullBankDetails(this Personnel personnel)
    {
        if (string.IsNullOrEmpty(personnel.BankName) || 
            string.IsNullOrEmpty(personnel.BankAccountNo))
        {
            return "Bank details not provided";
        }
        
        return $"{personnel.BankName} - {personnel.BankAccountNo}";
    }
}

public static class SalesExtensions
{
    public static bool IsCurrentMonth(this Sales sales)
    {
        var now = DateTime.Now;
        return sales.ReportDate.Year == now.Year && 
               sales.ReportDate.Month == now.Month;
    }
    
    public static decimal GetCommissionAmount(this Sales sales, CommissionProfile profile)
    {
        return profile.CommissionFixed + (profile.CommissionPercentage * sales.SalesAmount);
    }
}
```

## Dependencies

```xml
<PackageReference Include="System.ComponentModel.Annotations" />
<PackageReference Include="AutoMapper" />
<PackageReference Include="Newtonsoft.Json" />
```

This library serves as the foundation for all data contracts and domain logic throughout the Marketing Personnel Management System.
