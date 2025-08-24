# Deployment Guide

## Prerequisites

- Windows Server with IIS 8.0+
- SQL Server 2016 or later
- .NET 6.0+ Runtime (or .NET Core 3.1+)
- Administrative access to server

## Database Setup

### 1. Create Database
```sql
-- Connect to SQL Server using SQL Server Management Studio or sqlcmd
CREATE DATABASE Marketing;
```

### 2. Run Database Scripts
Execute scripts in the following order:
```bash
# Navigate to SQL folder
cd SQL/

# Run table creation script
sqlcmd -S localhost -d Marketing -i create_tables.sql

# Run seed data script
sqlcmd -S localhost -d Marketing -i seed_data.sql

# Optional: Run reporting views/procedures
sqlcmd -S localhost -d Marketing -i reporting_objects.sql
```

## IIS Configuration

### 1. Install Required Features
Enable the following Windows features:
- IIS Management Console
- ASP.NET Core Module
- .NET Core Hosting Bundle

### 2. Create Application Pool
```powershell
# Create application pool for API
New-WebAppPool -Name "MarketingAPI" -Force
Set-ItemProperty -Path "IIS:\AppPools\MarketingAPI" -Name processModel.identityType -Value ApplicationPoolIdentity
Set-ItemProperty -Path "IIS:\AppPools\MarketingAPI" -Name managedRuntimeVersion -Value ""

# Create application pool for WebUI
New-WebAppPool -Name "MarketingWebUI" -Force
Set-ItemProperty -Path "IIS:\AppPools\MarketingWebUI" -Name processModel.identityType -Value ApplicationPoolIdentity
Set-ItemProperty -Path "IIS:\AppPools\MarketingWebUI" -Name managedRuntimeVersion -Value ""
```

### 3. Create IIS Sites
```powershell
# Create API site
New-Website -Name "MarketingAPI" -Port 5001 -PhysicalPath "C:\inetpub\wwwroot\MarketingAPI" -ApplicationPool "MarketingAPI"

# Create WebUI site
New-Website -Name "MarketingWebUI" -Port 5000 -PhysicalPath "C:\inetpub\wwwroot\MarketingWebUI" -ApplicationPool "MarketingWebUI"
```

## Application Deployment

### 1. Copy Files
```bash
# Copy API files
xcopy /E /I Deploy\API\* C:\inetpub\wwwroot\MarketingAPI\

# Copy WebUI files
xcopy /E /I Deploy\WebUI\* C:\inetpub\wwwroot\MarketingWebUI\
```

### 2. Update Configuration
Edit `C:\inetpub\wwwroot\MarketingAPI\appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=Marketing;Integrated Security=true;TrustServerCertificate=True"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5000", "http://YOUR_WEBUI_URL"]
  }
}
```

### 3. Set Permissions
```powershell
# Grant IIS_IUSRS read/execute permissions
icacls "C:\inetpub\wwwroot\MarketingAPI" /grant "IIS_IUSRS:(OI)(CI)RX" /T
icacls "C:\inetpub\wwwroot\MarketingWebUI" /grant "IIS_IUSRS:(OI)(CI)RX" /T

# Grant application pool identity database access
# Run this SQL command replacing YOUR_SERVER with actual server name:
# CREATE LOGIN [IIS APPPOOL\MarketingAPI] FROM WINDOWS;
# USE Marketing;
# CREATE USER [IIS APPPOOL\MarketingAPI] FOR LOGIN [IIS APPPOOL\MarketingAPI];
# ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\MarketingAPI];
# ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\MarketingAPI];
```

## Configuration Updates

### API Configuration (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PRODUCTION_SERVER;Database=Marketing;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Cors": {
    "AllowedOrigins": ["http://your-webui-domain.com"]
  }
}
```

### WebUI Configuration
Update API base URL in `wwwroot/js/config.js`:
```javascript
const API_BASE_URL = 'http://your-api-domain.com:5001/api';
```

## Verification Steps

### 1. Test Database Connection
```sql
-- Verify tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo';

-- Verify seed data
SELECT COUNT(*) FROM Personnel;
SELECT COUNT(*) FROM Sales;
SELECT COUNT(*) FROM CommissionProfile;
```

### 2. Test API Endpoints
```bash
# Test API health
curl http://localhost:5001/api/personnel

# Expected: JSON array of personnel records
```

### 3. Test WebUI
1. Open browser to `http://localhost:5000`
2. Verify personnel grid loads
3. Test add/edit/delete operations
4. Verify sales visualization
5. Test reports functionality

## Troubleshooting

### Common Issues

#### API Returns 500 Error
- Check connection string in appsettings.json
- Verify database permissions for application pool identity
- Check Windows Event Log for detailed errors

#### CORS Errors
- Verify AllowedOrigins in API configuration
- Ensure WebUI URL matches exactly (including protocol and port)

#### Database Connection Fails
- Test connection string with SQL Server Management Studio
- Verify SQL Server is running and accepting connections
- Check firewall settings

#### Missing Dependencies
- Install .NET Core Hosting Bundle
- Restart IIS after installing runtime
- Verify correct runtime version

### Log Locations
- IIS Logs: `C:\inetpub\logs\LogFiles\`
- Application Logs: Windows Event Viewer → Application
- API Logs: Check configured logging provider

## Browser Compatibility

Tested and verified on:
- Chrome 90+
- Firefox 88+
- Edge 90+

## Performance Optimization

### IIS Settings
```xml
<!-- Add to web.config -->
<system.webServer>
  <staticContent>
    <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="7.00:00:00" />
  </staticContent>
  <httpCompression>
    <dynamicTypes>
      <add mimeType="application/json" enabled="true" />
    </dynamicTypes>
  </httpCompression>
</system.webServer>
```

### Database Optimization
- Ensure proper indexing on foreign keys
- Monitor query performance
- Consider connection pooling settings

## Security Checklist

- [ ] Database uses integrated security or strong passwords
- [ ] CORS configured for specific origins only
- [ ] HTTPS enabled in production
- [ ] Application pool runs with minimal privileges
- [ ] Error pages don't expose sensitive information
- [ ] Input validation enabled on all endpoints

## Maintenance

### Regular Tasks
- Monitor application logs for errors
- Backup database regularly
- Update .NET runtime as needed
- Monitor disk space and performance

### Updates
1. Stop IIS sites
2. Backup current deployment
3. Deploy new files
4. Update database schema if needed
5. Start IIS sites
6. Verify functionality
