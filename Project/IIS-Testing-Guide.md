# IIS Testing and Configuration Guide

## Prerequisites
- Windows Server 2016+ or Windows 10+ with IIS enabled
- .NET 9.0 Runtime installed
- SQL Server accessible from IIS server

## IIS Features Required
Enable these features in Windows Features:
- Internet Information Services
- World Wide Web Services
- Application Development Features
  - ASP.NET Core Module
  - .NET Extensibility 4.8
  - ASP.NET 4.8
- Common HTTP Features
  - HTTP Redirection
  - Static Content
- Security
  - Request Filtering

## Application Pool Configuration

### API Application Pool
```
Name: CompanyA_API_Pool
.NET CLR Version: No Managed Code
Managed Pipeline Mode: Integrated
Identity: ApplicationPoolIdentity
Enable 32-Bit Applications: False
```

### WebUI Application Pool
```
Name: CompanyA_WebUI_Pool
.NET CLR Version: No Managed Code
Managed Pipeline Mode: Integrated
Identity: ApplicationPoolIdentity
Enable 32-Bit Applications: False
```

## Site Configuration

### API Site
```
Site Name: CompanyA.API
Physical Path: C:\inetpub\wwwroot\CompanyA.API\
Port: 8080
Application Pool: CompanyA_API_Pool
```

### WebUI Site
```
Site Name: CompanyA.WebUI
Physical Path: C:\inetpub\wwwroot\CompanyA.WebUI\
Port: 80
Application Pool: CompanyA_WebUI_Pool
```

## MIME Types Configuration
Add these MIME types if not present:

| Extension | MIME Type |
|-----------|-----------|
| .json | application/json |
| .woff | font/woff |
| .woff2 | font/woff2 |
| .js | application/javascript |
| .map | application/json |

## URL Rewrite Module
Install URL Rewrite Module 2.1 from Microsoft:
https://www.iis.net/downloads/microsoft/url-rewrite

## Testing Checklist

### 1. Basic Connectivity
- [ ] API responds at http://localhost:8080/api/personnel
- [ ] WebUI loads at http://localhost
- [ ] No 500 errors in browser console
- [ ] No 404 errors for static resources

### 2. API Endpoints
Test these endpoints:
- [ ] GET /api/personnel
- [ ] GET /api/sales
- [ ] GET /api/commissionprofile
- [ ] GET /api/reports/management-overview
- [ ] GET /api/reports/commission-payout

### 3. CORS Configuration
- [ ] API accepts requests from WebUI domain
- [ ] Preflight OPTIONS requests work
- [ ] No CORS errors in browser console

### 4. Static File Serving
- [ ] CSS files load correctly
- [ ] JavaScript files load correctly
- [ ] Bootstrap icons display
- [ ] Chart.js loads and renders

### 5. SPA Routing
- [ ] Direct navigation to /Personnel works
- [ ] Direct navigation to /Sales works
- [ ] Direct navigation to /Reports works
- [ ] Browser back/forward buttons work

### 6. File Downloads
- [ ] CSV downloads work from Reports page
- [ ] Files download with correct MIME type
- [ ] No security warnings for downloads

## Common IIS Issues and Solutions

### Issue: 500.30 ASP.NET Core app failed to start
**Solution**: 
- Check .NET 9.0 Runtime is installed
- Verify application pool identity has permissions
- Check stdout logs in logs folder

### Issue: 404 for API endpoints
**Solution**:
- Verify ASP.NET Core Module is installed
- Check web.config aspNetCore configuration
- Ensure API site is running

### Issue: SPA routes return 404
**Solution**:
- Install URL Rewrite Module
- Verify web.config rewrite rules
- Check static file handling

### Issue: CORS errors
**Solution**:
- Update appsettings.json CORS policy
- Add CORS headers to web.config
- Verify API and WebUI domains match

### Issue: Static files not loading
**Solution**:
- Check MIME types configuration
- Verify file permissions
- Enable static content in IIS features

## Performance Optimization

### Compression
Enable in web.config:
```xml
<urlCompression doStaticCompression="true" doDynamicCompression="true" />
```

### Caching
Configure static content caching:
```xml
<clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="30.00:00:00" />
```

### Connection Pooling
Optimize database connections in appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MarketingPersonnelDB;Trusted_Connection=true;MultipleActiveResultSets=true;Pooling=true;Max Pool Size=100;"
  }
}
```

## Security Configuration

### SSL/TLS (Production)
- Install SSL certificate
- Redirect HTTP to HTTPS
- Update CORS policy for HTTPS URLs

### Security Headers
Already configured in web.config:
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY/SAMEORIGIN
- X-XSS-Protection: 1; mode=block

### File Permissions
Set appropriate permissions:
- IIS_IUSRS: Read & Execute
- Application Pool Identity: Read & Execute
- SYSTEM: Full Control

## Monitoring and Logs

### Log Locations
- **IIS Logs**: `C:\inetpub\logs\LogFiles\W3SVC[site-id]\`
- **Application Logs**: Windows Event Viewer → Application
- **ASP.NET Core Logs**: Application folder `\logs\` directory

### Key Metrics to Monitor
- Response times
- Error rates
- Memory usage
- CPU utilization
- Database connection pool

## Testing Commands

### PowerShell Testing
```powershell
# Test API connectivity
Invoke-RestMethod -Uri "http://localhost:8080/api/personnel" -Method GET

# Test WebUI connectivity
Invoke-WebRequest -Uri "http://localhost" -UseBasicParsing

# Check application pools
Get-IISAppPool | Where-Object {$_.Name -like "*CompanyA*"}

# Check sites
Get-IISSite | Where-Object {$_.Name -like "*CompanyA*"}
```

### Browser Testing
1. Open multiple browsers (Chrome, Firefox, Edge)
2. Navigate to http://localhost
3. Test all functionality per Cross-Browser-Testing-Checklist.md
4. Check browser developer tools for errors

## Deployment Verification
- [ ] Database connection successful
- [ ] All API endpoints respond correctly
- [ ] WebUI loads and functions properly
- [ ] Reports generate and download correctly
- [ ] Cross-browser compatibility confirmed
- [ ] Performance meets requirements
- [ ] Security headers present
- [ ] Logs are being generated
