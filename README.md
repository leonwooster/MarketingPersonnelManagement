# Marketing Personnel Management System

A comprehensive web-based system for managing marketing personnel, sales tracking, commission calculations, and reporting.

## Overview

The Marketing Personnel Management System is a modern web application built with ASP.NET Core that provides complete functionality for managing marketing teams, tracking sales performance, calculating commissions, and generating detailed reports. The system features a clean separation of concerns with a layered architecture and supports both web UI and REST API access.

## Features

### Core Functionality
- **Personnel Management**: Complete CRUD operations for marketing personnel
- **Sales Tracking**: Add and delete sales records with date and amount validation
- **Commission Profiles**: Configurable commission structures (fixed + percentage)
- **Reporting**: Management overview and commission payout reports
- **Data Visualization**: Interactive charts and graphs using Chart.js

### Upcoming Features (Sprint 4)
- **Multi-Manager Authentication**: Username/password login system
- **Data Isolation**: Manager-specific data access and filtering
- **Session Management**: Secure login sessions with timeout handling
- **Role-Based Access**: Manager-specific personnel and sales data

## Architecture

### Project Structure
```
MarketingPersonnelManagement/
├── CompanyA.API/                   # REST API (.NET 9.0)
├── CompanyA.WebUI/                 # Web Interface (ASP.NET Core MVC)
├── CompanyA.BusinessComponents/    # Business Logic Layer
├── CompanyA.BusinessEntity/        # DTOs and Data Contracts
├── CompanyA.DataAccess/           # Entity Framework Core Data Layer
├── Documentation/                  # Project Documentation
├── SQL/                           # Database Scripts
└── README.md                      # This file
```

### Technology Stack
- **Backend**: ASP.NET Core Web API (.NET 9.0)
- **Frontend**: ASP.NET Core MVC with Bootstrap 5 + Chart.js
- **Database**: SQL Server 2016+ with Entity Framework Core
- **Authentication**: ASP.NET Core Identity (Sprint 4)
- **API Documentation**: Swagger/OpenAPI
- **Deployment**: IIS with URL Rewrite

### Design Patterns
- **Layered Architecture**: Clear separation between presentation, business, and data layers
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Service registration and lifecycle management
- **DTO Pattern**: Data transfer objects for API communication
- **Single Page Application**: Modern web UI with AJAX interactions

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2016+ or SQL Server Express
- Visual Studio 2022 or VS Code
- IIS 10.0+ (for deployment)

### Development Setup
1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MarketingPersonnelManagement
   ```

2. **Configure database connection**
   ```json
   // CompanyA.API/appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Marketing;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
     }
   }
   ```

3. **Setup database**
   ```bash
   cd CompanyA.API
   dotnet ef database update
   ```

4. **Run the applications**
   ```bash
   # Terminal 1 - API
   cd CompanyA.API
   dotnet run

   # Terminal 2 - WebUI
   cd CompanyA.WebUI
   dotnet run
   ```

5. **Access the application**
   - WebUI: https://localhost:7000
   - API: https://localhost:7001
   - Swagger: https://localhost:7001/swagger

## API Endpoints

### Personnel Management
```
GET    /api/personnel              # List all personnel
GET    /api/personnel/{id}         # Get personnel by ID
POST   /api/personnel              # Create new personnel
PUT    /api/personnel/{id}         # Update personnel
DELETE /api/personnel/{id}?confirm=true  # Delete personnel
```

### Sales Management
```
GET    /api/sales?personnelId={id}&from={date}&to={date}
POST   /api/sales                  # Add sales record
DELETE /api/sales/{id}             # Delete sales record
```

### Commission Profiles
```
GET    /api/commissionprofiles     # List all profiles
GET    /api/commissionprofiles/{id} # Get profile by ID
POST   /api/commissionprofiles     # Create profile
PUT    /api/commissionprofiles/{id} # Update profile
DELETE /api/commissionprofiles/{id} # Delete profile
```

### Reports
```
GET    /api/reports/management?month={mm}&year={yyyy}
GET    /api/reports/commission?month={mm}&year={yyyy}
GET    /api/reports/export/{type}?format=csv
```

## Database Schema

### Core Tables
- **Personnel**: Marketing staff information with commission profile links
- **Sales**: Daily sales records linked to personnel
- **CommissionProfile**: Commission calculation configurations
- **Manager**: Authentication and data isolation (Sprint 4)

### Key Relationships
- Personnel → CommissionProfile (Many-to-One)
- Personnel → Sales (One-to-Many, Cascade Delete)
- Manager → Personnel (One-to-Many, Sprint 4)

### Business Rules
- Personnel age must be ≥ 19
- Sales amounts must be ≥ 0
- Sales dates cannot be in the future
- Commission profiles cannot be deleted if referenced by personnel

## Validation Rules

### Personnel
- **Name**: Required, 1-50 characters
- **Age**: Required, ≥ 19 years
- **Phone**: Required, 1-20 characters
- **Commission Profile**: Required, must exist
- **Bank Details**: Optional, max 20 characters each

### Sales
- **Personnel**: Required, must exist
- **Report Date**: Required, cannot be future date
- **Sales Amount**: Required, ≥ 0, decimal(10,2)

### Commission Profile
- **Profile Name**: Required, positive integer, unique
- **Commission Fixed**: Required, ≥ 0, decimal(10,2)
- **Commission Percentage**: Required, ≥ 0, decimal(10,6)

## Deployment

### IIS Deployment Guide

1. **Build for Production**
   ```bash
   # API
   cd CompanyA.API
   dotnet publish -c Release -o C:\inetpub\CompanyA.API

   # WebUI
   cd CompanyA.WebUI
   dotnet publish -c Release -o C:\inetpub\CompanyA.WebUI
   ```

2. **IIS Configuration**
   - Install ASP.NET Core Hosting Bundle
   - Create Application Pools (No Managed Code, Integrated Pipeline)
   - Create IIS Sites pointing to published folders
   - Configure SSL certificates for HTTPS

3. **Database Setup**
   ```bash
   # Update connection string in appsettings.json
   # Run migrations on production database
   dotnet ef database update --connection "Production Connection String"
   ```

4. **Verify Deployment**
   - Test API endpoints via Swagger
   - Verify WebUI functionality
   - Check CORS configuration
   - Validate static file serving

### Configuration Files
- `web.config`: Auto-generated with security headers and MIME types
- `appsettings.json`: Connection strings and application settings
- URL Rewrite rules for SPA routing

## Development Guidelines

### Code Standards
- Follow SOLID principles
- Use async/await patterns
- Implement proper error handling
- Write comprehensive unit tests
- Document public APIs

### Git Workflow
- Feature branches for new development
- Pull requests for code review
- Automated testing in CI/CD
- Semantic versioning for releases

### Testing Strategy
- Unit tests for business logic
- Integration tests for API endpoints
- UI tests for critical user flows
- Performance tests for large datasets

## Security

### Current Implementation
- Input validation on all endpoints
- SQL injection prevention via EF Core
- XSS protection in web UI
- CORS configuration for API access
- Security headers in web.config

### Sprint 4 Security Features
- Password hashing with BCrypt
- Session-based authentication
- Manager-specific data isolation
- Secure cookie configuration
- Brute force protection

## Performance

### Optimization Strategies
- Async database operations
- Efficient EF Core queries with proper includes
- Client-side caching for static data
- Response compression for large datasets
- Connection pooling for database access

### Monitoring
- Application performance monitoring
- Database query performance tracking
- Error logging and alerting
- User activity analytics

## Browser Compatibility

### Supported Browsers
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

### Features
- Responsive design for mobile/tablet
- Progressive enhancement
- Accessibility compliance (WCAG 2.1)
- Cross-browser testing

## Troubleshooting

### Common Issues

**IIS Deployment**
- HTTP 500.19: Check web.config for duplicate MIME types
- CORS errors: Verify API CORS policy configuration
- Static files not loading: Check MIME type mappings
- Database connection: Verify connection string and permissions

**Development**
- Migration errors: Ensure EF Core tools are installed
- API not accessible: Check HTTPS certificates and ports
- JavaScript errors: Verify Chart.js and Bootstrap loading
- Authentication issues: Check cookie settings and session configuration

### Debug Tools
- Browser developer tools for client-side issues
- Application logs for server-side errors
- SQL Server Profiler for database queries
- IIS logs for deployment issues

## Contributing

### Development Process
1. Fork the repository
2. Create a feature branch
3. Implement changes with tests
4. Submit pull request
5. Code review and merge

### Coding Standards
- Use consistent naming conventions
- Follow C# and JavaScript style guides
- Write meaningful commit messages
- Include unit tests for new features
- Update documentation as needed

## Documentation

### Available Documentation
- [API Documentation](CompanyA.API/README.md)
- [WebUI Documentation](CompanyA.WebUI/README.md)
- [Data Access Documentation](CompanyA.DataAccess/README.md)
- [Business Components Documentation](CompanyA.BusinessComponents/README.md)
- [Business Entities Documentation](CompanyA.BusinessEntity/README.md)
- [Architecture Guide](Documentation/Architecture.md)
- [Deployment Guide](Documentation/Deploy-Guide.md)

### Sprint Documentation
- Sprint 1: Core API and basic UI
- Sprint 2: Sales management and commission profiles
- Sprint 3: Reporting and deployment
- Sprint 4: Authentication and data isolation (upcoming)

## Support

### Getting Help
- Check documentation in `/Documentation` folder
- Review README files in each project
- Check GitHub issues for known problems
- Contact development team for support

### Reporting Issues
- Use GitHub issues for bug reports
- Include steps to reproduce
- Provide error messages and logs
- Specify browser and environment details

## License

This project is proprietary software developed for internal use. All rights reserved.

## Changelog

### Version 1.3.0 (Current)
- ✅ IIS deployment configuration resolved
- ✅ CORS policy fixes for API access
- ✅ Static file serving improvements
- ✅ Navigation routing fixes for SPA
- ✅ Manager entity preparation for Sprint 4
- ✅ Comprehensive README documentation updates

### Version 1.2.0
- ✅ Reports functionality (management overview, commission payout)
- ✅ CSV export capabilities
- ✅ Chart.js integration for data visualization
- ✅ Cross-browser compatibility testing

### Version 1.1.0
- ✅ Sales management with grid/graph toggle
- ✅ Commission profile CRUD operations
- ✅ Data validation and business rules
- ✅ API endpoint completion

### Version 1.0.0
- ✅ Initial project structure
- ✅ Personnel CRUD operations
- ✅ Basic UI with Bootstrap 5
- ✅ Entity Framework Core setup
- ✅ REST API foundation

### Upcoming Version 2.0.0 (Sprint 4)
- 🔄 Multi-manager authentication system
- 🔄 Manager-specific data isolation
- 🔄 Login/logout functionality
- 🔄 Session management
- 🔄 Enhanced security features