# Marketing Personnel Management System

A single-page web application for managing marketing department personnel with sales tracking and commission reporting capabilities.

## Overview

This solution provides a comprehensive system for:
- Managing marketing personnel (CRUD operations)
- Tracking daily sales records
- Viewing sales data in grid/graph format
- Generating management and finance reports
- Calculating commission payouts

## Architecture

The system follows a layered architecture with REST microservices:

```
┌─────────────────┐    ┌─────────────────┐
│   CompanyA.     │    │   CompanyA.     │
│     WebUI       │◄──►│      API        │
│  (SPA Client)   │    │ (REST Services) │
└─────────────────┘    └─────────────────┘
                              │
                       ┌─────────────────┐
                       │   CompanyA.     │
                       │Business         │
                       │Components       │
                       └─────────────────┘
                              │
                       ┌─────────────────┐
                       │   CompanyA.     │
                       │  DataAccess     │
                       │  (EF Core)      │
                       └─────────────────┘
                              │
                       ┌─────────────────┐
                       │   SQL Server    │
                       │   Database      │
                       │  (Marketing)    │
                       └─────────────────┘
```

## Projects

| Project | Purpose | Technology |
|---------|---------|------------|
| **CompanyA.WebUI** | Single-page application hosting | ASP.NET Core MVC/Razor Pages |
| **CompanyA.API** | REST microservices | ASP.NET Core Web API |
| **CompanyA.BusinessComponents** | Business logic layer | .NET Class Library |
| **CompanyA.DataAccess** | Data access layer | EF Core |
| **CompanyA.BusinessEntity** | Domain models/DTOs | .NET Class Library |

## Technology Stack

- **Backend**: .NET Core/6+, ASP.NET Core Web API, Entity Framework Core
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5, Chart.js
- **Database**: SQL Server 2016+
- **Hosting**: IIS
- **Architecture**: REST microservices, Single Page Application

## Key Features

### Personnel Management
- Add, edit, delete personnel with validation
- Required fields: name, age (≥19), phone, commission profile
- Cascade delete of sales records when personnel is deleted
- Confirmation prompt for deletions

### Sales Tracking
- View monthly sales for selected personnel
- Toggle between grid and graph visualization
- Add/delete sales records (no editing allowed)
- Handle missing days (no sales data expected for some days)

### Reporting
- **Management Reports**: Performance overview, top performers, monthly totals
- **Finance Reports**: Commission payout calculations based on profiles
- Export capabilities (CSV format)

### Commission Profiles
- Define fixed commission amounts and percentage rates
- Link personnel to commission profiles
- Calculate monthly payouts: `fixed + (percentage × monthly_sales)`

## Database Schema

### Personnel
- `Id` int (PK)
- `name` varchar(50) - Required
- `age` int - Required (≥19)
- `phone` varchar(20) - Required
- `commission_profile_id` int (FK) - Required
- `bank_name` varchar(20)
- `bank_account_no` varchar(20)

### Sales
- `Id` int (PK)
- `personnel_id` int (FK → Personnel.Id)
- `report_date` datetime
- `sales_amount` decimal(10,2)

### CommissionProfile
- `Id` int (PK)
- `profile_name` int
- `commission_fixed` decimal(10,2)
- `commission_percentage` decimal(10,6)

## Getting Started

1. **Database Setup**: Run SQL scripts from `SQL/` folder
2. **Configuration**: Update connection strings in `appsettings.json`
3. **Build**: `dotnet build CompanyA.WebUI/CompanyA.WebUI.sln`
4. **Deploy**: Use artifacts from `Deploy/` folder for IIS deployment

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Edge (latest)

## Security Considerations

- Server-side validation on all inputs
- No authentication required (as per requirements)
- Connection strings via configuration (not hardcoded)
- Input sanitization at API boundary

## Development Guidelines

- Follow exact database schema specifications
- Implement server-side validation: name required, age ≥19, phone non-empty
- No sales edit endpoint - only add/delete operations
- Personnel deletion requires confirmation and cascades to sales
- WebUI must only consume REST API endpoints (no direct database access)

## Folder Structure

```
Project/
├── SQL/              # Database scripts
├── Deploy/           # IIS deployment artifacts
├── Source/           # Clean source code
└── Documentation/    # Architecture and design docs
```

For detailed deployment instructions, see `Deploy_Guide.md`.
