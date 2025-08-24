# Cross-Browser Testing Checklist

## Testing Environment Setup
- Test on Windows 10/11 with latest browser versions
- Clear browser cache before each test session
- Disable browser extensions that might interfere

## Browser Compatibility Matrix

### ✅ Chrome (Latest)
- **JavaScript**: ES5 syntax with jQuery - ✅ Compatible
- **CSS**: Bootstrap 5 - ✅ Compatible
- **AJAX**: jQuery AJAX - ✅ Compatible
- **Charts**: Chart.js - ✅ Compatible

### ✅ Firefox (Latest)
- **JavaScript**: ES5 syntax with jQuery - ✅ Compatible
- **CSS**: Bootstrap 5 - ✅ Compatible
- **AJAX**: jQuery AJAX - ✅ Compatible
- **Charts**: Chart.js - ✅ Compatible

### ✅ Edge (Latest)
- **JavaScript**: ES5 syntax with jQuery - ✅ Compatible
- **CSS**: Bootstrap 5 - ✅ Compatible
- **AJAX**: jQuery AJAX - ✅ Compatible
- **Charts**: Chart.js - ✅ Compatible

### ⚠️ Internet Explorer 11
- **JavaScript**: ES5 syntax with jQuery - ✅ Compatible
- **CSS**: Bootstrap 5 - ⚠️ Limited support (fallbacks needed)
- **AJAX**: jQuery AJAX - ✅ Compatible
- **Charts**: Chart.js - ✅ Compatible

## Functional Testing Checklist

### 1. Navigation & Layout
- [ ] Header navigation displays correctly
- [ ] Active page highlighting works
- [ ] Responsive design on different screen sizes
- [ ] Bootstrap icons display properly

### 2. Personnel Management
- [ ] Personnel list loads and displays
- [ ] Add new personnel form validation
- [ ] Edit personnel functionality
- [ ] Delete personnel with confirmation
- [ ] Commission profile dropdown works

### 3. Sales Management
- [ ] Personnel selection dropdown populates
- [ ] Date filters work correctly
- [ ] Sales grid displays data
- [ ] Chart view toggles properly
- [ ] Add sales form validation
- [ ] Delete sales functionality

### 4. Reports Section
- [ ] Management Overview loads
- [ ] Commission Payout loads
- [ ] Year/Month filters work
- [ ] Personnel filter works
- [ ] CSV download buttons function
- [ ] Downloaded CSV files open correctly

### 5. API Integration
- [ ] All AJAX calls complete successfully
- [ ] Error handling displays user-friendly messages
- [ ] Loading states show appropriately
- [ ] CORS headers work across domains

## Browser-Specific Issues to Watch For

### Chrome
- Generally most compatible
- Test CSV download behavior

### Firefox
- Check file download permissions
- Verify Chart.js rendering

### Edge
- Test legacy Edge vs Chromium Edge
- Verify CORS handling

### Internet Explorer 11
- **Known Limitations**:
  - Bootstrap 5 has limited IE11 support
  - Some CSS Grid features not supported
  - Modern JavaScript polyfills may be needed
- **Fallback Strategy**:
  - Use Bootstrap 4 for IE11 if needed
  - Provide graceful degradation message

## Performance Testing
- [ ] Page load times under 3 seconds
- [ ] Chart rendering performance
- [ ] Large dataset handling (100+ records)
- [ ] Memory usage stays reasonable

## Security Testing
- [ ] XSS protection headers present
- [ ] CSRF tokens working
- [ ] SQL injection prevention
- [ ] File download security

## Testing Commands

### Start Application for Testing
```bash
# Terminal 1 - API
cd CompanyA.API
dotnet run

# Terminal 2 - WebUI  
cd CompanyA.WebUI
dotnet run
```

### Browser Testing URLs
- **WebUI**: http://localhost:5000
- **API**: http://localhost:5001
- **API Swagger**: http://localhost:5001/swagger

## Test Data Setup
Use the provided SQL scripts:
1. Run `create_tables.sql`
2. Run `seed_data.sql`
3. Optionally run `reporting_views.sql`

## Issue Reporting Template
```
**Browser**: [Chrome/Firefox/Edge/IE11]
**Version**: [Browser version]
**Issue**: [Description]
**Steps to Reproduce**: 
1. 
2. 
3. 
**Expected**: [What should happen]
**Actual**: [What actually happens]
**Console Errors**: [Any JavaScript errors]
```
