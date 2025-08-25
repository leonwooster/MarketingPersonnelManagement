# Marketing Personnel Management System - Deployment Guide

## Overview
This guide provides step-by-step instructions for deploying the Marketing Personnel Management System to a production IIS environment.

## System Requirements

### Server Requirements
- Windows Server 2019 or later
- IIS 10.0 or later
- .NET 9.0 Runtime
- SQL Server 2019 or later

### Client Requirements
- Modern web browser (Chrome 90+, Firefox 88+, Edge 90+, Safari 14+)
- JavaScript enabled
- Internet connection for CDN resources (Bootstrap, Chart.js)

## Deployment Structure

```
Project/
├── SQL/                    # Database scripts
│   ├── create_tables.sql   # Database schema creation
│   ├── seed_data.sql      # Initial data population
│   └── reporting_views.sql # Optional reporting views
├── Deploy/                 # IIS-ready artifacts
│   ├── API/               # Web API deployment files
│   └── WebUI/             # Web UI deployment files
└── Source/                 # Clean source code
```

## Step 1: Database Setup

### 1.1 Create Database
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Execute `SQL/create_tables.sql` to create the database schema
4. Execute `SQL/seed_data.sql` to populate initial data
5. Optionally execute `SQL/reporting_views.sql` for enhanced reporting

### 1.2 Verify Database
Run this query to verify successful setup:
```sql
USE MarketingPersonnelDB;
SELECT 
    'Commission Profiles' AS TableName, COUNT(*) AS Records FROM CommissionProfile
UNION ALL
SELECT 'Personnel' AS TableName, COUNT(*) AS Records FROM Personnel
UNION ALL
SELECT 'Sales Records' AS TableName, COUNT(*) AS Records FROM Sales;
```

Expected results:
- Commission Profiles: 3 records
- Personnel: 8 records  
- Sales Records: 64+ records

## Step 2: API Configuration

### 2.1 Connection String Setup
Update `appsettings.json` in the API deployment:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=MarketingPersonnelDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 2.2 CORS Configuration
For production, update CORS policy in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://your-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## Step 3: IIS Setup

### 3.1 Create Application Pool
1. Open IIS Manager
2. Right-click "Application Pools" → "Add Application Pool"
3. Name: `MarketingPersonnelAPI`
4. .NET CLR Version: `No Managed Code`
5. Managed Pipeline Mode: `Integrated`
6. Start Application Pool: `True`

### 3.2 Create API Site
1. Right-click "Sites" → "Add Website"
2. Site Name: `MarketingPersonnel.API`
3. Application Pool: `MarketingPersonnelAPI`
4. Physical Path: `C:\inetpub\wwwroot\MarketingPersonnel\API`
5. Port: `5041` (or your preferred port)
6. Host Name: Leave blank or set your domain

### 3.3 Create WebUI Application Pool
1. Right-click "Application Pools" → "Add Application Pool"
2. Name: `MarketingPersonnelWebUI`
3. .NET CLR Version: `No Managed Code`
4. Managed Pipeline Mode: `Integrated`

### 3.4 Create WebUI Site
1. Right-click "Sites" → "Add Website"
2. Site Name: `MarketingPersonnel.WebUI`
3. Application Pool: `MarketingPersonnelWebUI`
4. Physical Path: `C:\inetpub\wwwroot\MarketingPersonnel\WebUI`
5. Port: `80` (or `443` for HTTPS)
6. Host Name: Your domain name

## Step 4: File Deployment

### 4.1 Deploy API Files
1. Copy contents of `Deploy/API/` to `C:\inetpub\wwwroot\MarketingPersonnel\API\`
2. Ensure IIS_IUSRS has read permissions on the folder
3. Verify `web.config` exists and is properly configured

### 4.2 Deploy WebUI Files
1. Copy contents of `Deploy/WebUI/` to `C:\inetpub\wwwroot\MarketingPersonnel\WebUI\`
2. Ensure IIS_IUSRS has read permissions on the folder

### 4.3 Configure MIME Types
Add these MIME types in IIS if not already present:
- `.json` → `application/json`
- `.woff` → `font/woff`
- `.woff2` → `font/woff2`

## Step 5: URL Rewrite Configuration

### 5.1 Install URL Rewrite Module
Download and install IIS URL Rewrite Module 2.1 from Microsoft.

### 5.2 Configure SPA Routing
Add this `web.config` to the WebUI root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="SPA Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/api/" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
    <staticContent>
      <mimeMap fileExtension=".json" mimeType="application/json" />
    </staticContent>
  </system.webServer>
</configuration>
```

## Step 6: Security Configuration

### 6.1 SSL Certificate (Recommended)
1. Obtain SSL certificate for your domain
2. Install certificate in IIS
3. Add HTTPS binding to WebUI site
4. Update API base URL in WebUI configuration

### 6.2 Firewall Rules
Ensure these ports are open:
- Port 80 (HTTP) or 443 (HTTPS) for WebUI
- Port 5041 (or your chosen port) for API

## Step 7: Testing Deployment

### 7.1 API Health Check
Navigate to: `http://your-api-domain:5041/api/personnel`
Expected: JSON response with personnel data

### 7.2 WebUI Access
Navigate to: `http://your-webui-domain/`
Expected: Marketing Personnel Management homepage

### 7.3 Cross-Browser Testing
Test the application in:
- ✅ Chrome (latest)
- ✅ Firefox (latest)
- ✅ Edge (latest)
- ✅ Safari (if applicable)

### 7.4 Functionality Testing
1. **Personnel Management**: Add, edit, delete personnel
2. **Sales Management**: Add, delete sales records
3. **Reports**: Generate management overview and commission reports
4. **CSV Export**: Download reports as CSV files

## Troubleshooting

### Common Issues

**Issue**: API returns 500 errors
**Solution**: Check connection string and database connectivity

**Issue**: WebUI shows blank page
**Solution**: Check browser console for JavaScript errors, verify CDN resources load

**Issue**: CORS errors in browser
**Solution**: Verify CORS policy includes your WebUI domain

**Issue**: CSV downloads fail
**Solution**: Check MIME type configuration and file permissions

### Log Locations
- API Logs: Windows Event Viewer → Application Logs
- IIS Logs: `C:\inetpub\logs\LogFiles\`

## Maintenance

### Regular Tasks
1. **Database Backup**: Schedule regular backups of MarketingPersonnelDB
2. **Log Monitoring**: Monitor IIS and application logs for errors
3. **Performance Monitoring**: Monitor CPU, memory, and disk usage
4. **Security Updates**: Keep Windows Server, IIS, and .NET runtime updated

### Scaling Considerations
- Consider load balancing for high traffic
- Implement database clustering for high availability
- Use CDN for static assets in global deployments

## Support
For technical support, contact the development team with:
- Error messages and screenshots
- Browser and version information
- Steps to reproduce issues
- Server logs if available

---
**Document Version**: 1.0  
**Last Updated**: March 2025  
**Deployment Target**: Production IIS Environment
