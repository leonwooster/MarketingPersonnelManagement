# Software Design Document

## Design Principles

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes must be substitutable for base classes
- **Interface Segregation**: Clients should not depend on unused interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

### Domain-Driven Design
- Clear domain boundaries between Personnel, Sales, and Commission domains
- Rich domain models with business logic encapsulation
- Repository pattern for data access abstraction

## API Design

### RESTful Endpoints

#### Personnel Service
```
GET    /api/personnel           # List all personnel
GET    /api/personnel/{id}      # Get personnel by ID
POST   /api/personnel           # Create new personnel
PUT    /api/personnel/{id}      # Update personnel
DELETE /api/personnel/{id}?confirm=true  # Delete with confirmation
```

#### Sales Service
```
GET    /api/sales?personnelId={id}&from={date}&to={date}  # Get sales by criteria
POST   /api/sales               # Add new sales record
DELETE /api/sales/{id}          # Delete sales record
```

#### Commission Profile Service
```
GET    /api/commissionprofiles  # List all profiles
GET    /api/commissionprofiles/{id}  # Get profile by ID
POST   /api/commissionprofiles  # Create new profile
PUT    /api/commissionprofiles/{id}  # Update profile
DELETE /api/commissionprofiles/{id}  # Delete profile
```

#### Reports Service
```
GET    /api/reports/management?month={mm}&year={yyyy}  # Management overview
GET    /api/reports/commission?month={mm}&year={yyyy}  # Commission payout
GET    /api/reports/export/{type}?format=csv          # Export reports
```

### Request/Response Models

#### Personnel DTO
```csharp
public class PersonnelDto
{
    public int Id { get; set; }
    public string Name { get; set; }        // Required, max 50 chars
    public int Age { get; set; }            // Required, >= 19
    public string Phone { get; set; }       // Required, max 20 chars
    public int CommissionProfileId { get; set; }  // Required, FK
    public string BankName { get; set; }    // Optional, max 20 chars
    public string BankAccountNo { get; set; }  // Optional, max 20 chars
}
```

#### Sales DTO
```csharp
public class SalesDto
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }    // Required, FK
    public DateTime ReportDate { get; set; } // Required, not future
    public decimal SalesAmount { get; set; } // Required, >= 0
}
```

#### Commission Profile DTO
```csharp
public class CommissionProfileDto
{
    public int Id { get; set; }
    public int ProfileName { get; set; }    // Required
    public decimal CommissionFixed { get; set; }     // decimal(10,2)
    public decimal CommissionPercentage { get; set; } // decimal(10,6)
}
```

### Validation Rules

#### Personnel Validation
- `Name`: Required, 1-50 characters, non-empty after trim
- `Age`: Required, integer >= 19
- `Phone`: Required, 1-20 characters, non-empty after trim
- `CommissionProfileId`: Required, must exist in CommissionProfile table
- `BankName`: Optional, max 20 characters
- `BankAccountNo`: Optional, max 20 characters

#### Sales Validation
- `PersonnelId`: Required, must exist in Personnel table
- `ReportDate`: Required, cannot be future date
- `SalesAmount`: Required, decimal >= 0, max precision (10,2)

#### Commission Profile Validation
- `ProfileName`: Required, positive integer
- `CommissionFixed`: Required, decimal(10,2), >= 0
- `CommissionPercentage`: Required, decimal(10,6), >= 0, <= 1

## Database Design

### Entity Relationships
```
CommissionProfile (1) ←→ (N) Personnel (1) ←→ (N) Sales
```

### Constraints and Indexes
```sql
-- Primary Keys
ALTER TABLE Personnel ADD CONSTRAINT PK_Personnel PRIMARY KEY (Id)
ALTER TABLE Sales ADD CONSTRAINT PK_Sales PRIMARY KEY (Id)
ALTER TABLE CommissionProfile ADD CONSTRAINT PK_CommissionProfile PRIMARY KEY (Id)

-- Foreign Keys
ALTER TABLE Personnel ADD CONSTRAINT FK_Personnel_CommissionProfile 
    FOREIGN KEY (commission_profile_id) REFERENCES CommissionProfile(Id)
    
ALTER TABLE Sales ADD CONSTRAINT FK_Sales_Personnel 
    FOREIGN KEY (personnel_id) REFERENCES Personnel(Id) ON DELETE CASCADE

-- Indexes for Performance
CREATE INDEX IX_Sales_PersonnelId ON Sales(personnel_id)
CREATE INDEX IX_Sales_ReportDate ON Sales(report_date)
CREATE INDEX IX_Personnel_CommissionProfileId ON Personnel(commission_profile_id)
```

### Data Integrity Rules
- Personnel cannot be deleted if referenced by active sales (handled by CASCADE)
- Commission profiles cannot be deleted if referenced by personnel
- Sales records cannot have future dates
- All monetary values must be non-negative

## Frontend Design

### Component Architecture
```
App.js (Main SPA Controller)
├── PersonnelGrid.js (Personnel management)
├── PersonnelModal.js (Add/Edit forms)
├── SalesPanel.js (Sales visualization)
├── SalesChart.js (Chart.js wrapper)
├── ReportsSection.js (Reports display)
└── ApiClient.js (HTTP service layer)
```

### State Management
- Local component state for UI interactions
- API calls for data persistence
- Event-driven updates between components

### UI/UX Design Patterns
- Modal dialogs for forms
- Confirmation dialogs for destructive actions
- Loading states during API calls
- Error message display
- Responsive grid layouts

## Business Logic Design

### Commission Calculation
```csharp
public decimal CalculateMonthlyCommission(int personnelId, int month, int year)
{
    var personnel = GetPersonnel(personnelId);
    var profile = GetCommissionProfile(personnel.CommissionProfileId);
    var monthlySales = GetMonthlySales(personnelId, month, year);
    
    return profile.CommissionFixed + (profile.CommissionPercentage * monthlySales);
}
```

### Sales Aggregation
```csharp
public MonthlyReport GetMonthlyReport(int month, int year)
{
    return new MonthlyReport
    {
        TotalSales = GetTotalSales(month, year),
        TopPerformers = GetTopPerformers(month, year, 5),
        AveragePerPerson = GetAveragePerPerson(month, year),
        DaysWithoutSales = GetDaysWithoutSales(month, year)
    };
}
```

## Error Handling Design

### API Error Responses
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

### Frontend Error Handling
- Form validation with real-time feedback
- Network error recovery
- User-friendly error messages
- Graceful degradation

## Testing Strategy

### Unit Testing
- Business logic validation
- Calculation accuracy
- Data transformation

### Integration Testing
- API endpoint functionality
- Database operations
- End-to-end workflows

### UI Testing
- Form validation
- Grid operations
- Chart rendering
- Cross-browser compatibility

## Performance Considerations

### Database Optimization
- Efficient queries with proper joins
- Pagination for large datasets
- Connection pooling
- Query result caching

### Frontend Optimization
- Minimal HTTP requests
- Efficient DOM updates
- Lazy loading of data
- Responsive design patterns

### API Optimization
- Async/await patterns
- Proper HTTP status codes
- Compression for large responses
- Rate limiting considerations

## Security Design

### Input Validation
- Server-side validation (primary)
- Client-side validation (UX enhancement)
- SQL injection prevention
- XSS protection

### Data Protection
- Parameterized queries
- Output encoding
- Secure configuration management
- Error message sanitization

## Deployment Design

### Build Process
```bash
# Build solution
dotnet build CompanyA.WebUI/CompanyA.WebUI.sln -c Release

# Publish API
dotnet publish CompanyA.API/CompanyA.API.csproj -c Release -o Deploy/API

# Publish WebUI
dotnet publish CompanyA.WebUI/CompanyA.WebUI.csproj -c Release -o Deploy/WebUI
```

### Configuration Management
- Environment-specific appsettings
- Connection string externalization
- Feature flag support
- Logging configuration

### Monitoring and Diagnostics
- Application insights integration
- Health check endpoints
- Performance counters
- Error tracking
