# CompanyA.API

REST microservices API for the Marketing Personnel Management System.

## Overview

This project provides RESTful endpoints for managing marketing personnel, sales records, commission profiles, and generating reports. Built with ASP.NET Core Web API and Entity Framework Core.

## Technology Stack

- **Framework**: ASP.NET Core Web API (.NET 6+)
- **ORM**: Entity Framework Core
- **Database**: SQL Server 2016+
- **Serialization**: System.Text.Json
- **Validation**: Data Annotations + FluentValidation

## API Endpoints

### Personnel Management
```
GET    /api/personnel           # List all personnel
GET    /api/personnel/{id}      # Get personnel by ID
POST   /api/personnel           # Create new personnel
PUT    /api/personnel/{id}      # Update personnel
DELETE /api/personnel/{id}?confirm=true  # Delete with confirmation
```

### Sales Management
```
GET    /api/sales?personnelId={id}&from={date}&to={date}  # Get sales by criteria
POST   /api/sales               # Add new sales record
DELETE /api/sales/{id}          # Delete sales record
```

### Commission Profiles
```
GET    /api/commissionprofiles  # List all profiles
GET    /api/commissionprofiles/{id}  # Get profile by ID
POST   /api/commissionprofiles  # Create new profile
PUT    /api/commissionprofiles/{id}  # Update profile
DELETE /api/commissionprofiles/{id}  # Delete profile
```

### Reports
```
GET    /api/reports/management?month={mm}&year={yyyy}  # Management overview
GET    /api/reports/commission?month={mm}&year={yyyy}  # Commission payout
GET    /api/reports/export/{type}?format=csv          # Export reports
```

## Validation Rules

### Personnel
- **Name**: Required, 1-50 characters
- **Age**: Required, >= 19
- **Phone**: Required, 1-20 characters
- **CommissionProfileId**: Required, must exist
- **BankName**: Optional, max 20 characters
- **BankAccountNo**: Optional, max 20 characters

### Sales
- **PersonnelId**: Required, must exist
- **ReportDate**: Required, cannot be future date
- **SalesAmount**: Required, >= 0, decimal(10,2)

### Commission Profile
- **ProfileName**: Required, positive integer
- **CommissionFixed**: Required, decimal(10,2), >= 0
- **CommissionPercentage**: Required, decimal(10,6), >= 0

## Configuration

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Marketing;User Id=sa;Password=999999;TrustServerCertificate=True"
  }
}
```

### CORS Settings
```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:5000"]
  }
}
```

## Business Rules

1. **Personnel Deletion**: Requires confirmation flag and cascades to sales records
2. **Sales Editing**: Not allowed - only add/delete operations
3. **Commission Calculation**: `fixed + (percentage × monthly_sales)`
4. **Date Validation**: Sales records cannot have future dates
5. **Referential Integrity**: Foreign key constraints enforced

## Error Handling

### Validation Errors (400)
```json
{
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "One or more validation errors occurred",
    "details": [
      {
        "field": "age",
        "message": "Age must be 19 or older"
      }
    ]
  }
}
```

### Not Found (404)
```json
{
  "error": {
    "code": "NOT_FOUND",
    "message": "Personnel with ID 123 not found"
  }
}
```

## Development

### Running Locally
```bash
# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run application
dotnet run
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Generate SQL script
dotnet ef migrations script
```

### Testing
```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment

### Build for Production
```bash
dotnet publish -c Release -o ../Deploy/API
```

### IIS Configuration
- Target Framework: .NET 6.0+
- Application Pool: No Managed Code
- Enable CORS for WebUI origin

## Logging

Configured logging levels:
- **Development**: Information
- **Production**: Warning
- **Database**: Warning (EF Core queries)

Log locations:
- Console (Development)
- Windows Event Log (Production)
- File logging (configurable)

## Security

- Input validation on all endpoints
- SQL injection prevention via EF Core
- CORS configuration for allowed origins
- No authentication required (per requirements)

## Performance

- Async/await patterns throughout
- Efficient EF Core queries with proper includes
- Connection pooling enabled
- Response compression for large datasets

## Dependencies

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.AspNetCore.Cors" />
<PackageReference Include="FluentValidation.AspNetCore" />
```
