# CompanyA.WebUI

Web User Interface for the Marketing Personnel Management System.

## Overview

This project provides the web-based user interface for the Marketing Personnel Management System. Built as an ASP.NET Core MVC application hosting a Single Page Application (SPA) with Bootstrap 5 and vanilla JavaScript.

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 9.0)
- **Frontend**: Bootstrap 5, vanilla JavaScript, Chart.js
- **UI Pattern**: Single Page Application with Razor Pages
- **Styling**: Bootstrap 5 with custom CSS
- **Charts**: Chart.js for data visualization

## Project Structure

```
CompanyA.WebUI/
├── Pages/
│   ├── Shared/
│   │   ├── _Layout.cshtml          # Main layout template
│   │   └── _ViewImports.cshtml     # Global imports
│   ├── Index.cshtml                # Personnel Management (main page)
│   ├── Sales.cshtml                # Sales Management
│   ├── Reports.cshtml              # Reports Dashboard
│   ├── Privacy.cshtml              # Privacy Policy
│   └── Error.cshtml                # Error handling
├── wwwroot/
│   ├── css/
│   │   ├── bootstrap.min.css       # Bootstrap 5 styles
│   │   └── site.css                # Custom styles
│   ├── js/
│   │   ├── bootstrap.bundle.min.js # Bootstrap 5 JavaScript
│   │   ├── chart.min.js            # Chart.js library
│   │   └── site.js                 # Custom JavaScript
│   └── lib/                        # Third-party libraries
├── web.config                      # IIS configuration
└── Program.cs                      # Application startup
```

## Pages and Features

### Index.cshtml - Personnel Management
Main landing page for personnel operations.

**Features**:
- Personnel list with search and filtering
- Add/Edit personnel modal forms
- Delete confirmation dialogs
- Commission profile selection
- Responsive data table
- Real-time validation

**UI Components**:
- Bootstrap data table with sorting
- Modal forms for CRUD operations
- Toast notifications for feedback
- Loading spinners for async operations

### Sales.cshtml - Sales Management
Sales tracking and visualization page.

**Features**:
- Personnel selection dropdown
- Date range filtering (defaults to current month)
- Sales data grid with add/delete operations
- Chart visualization toggle (grid ↔ graph)
- Monthly sales summaries
- Export functionality

**UI Components**:
- Chart.js line/bar charts
- Bootstrap form controls
- Dynamic data loading
- Toggle buttons for view switching

### Reports.cshtml - Reports Dashboard
Management and finance reporting interface.

**Features**:
- Management overview reports
- Commission payout calculations
- Month/year filtering
- Top performers display
- CSV export functionality
- Interactive charts and summaries

**UI Components**:
- Dashboard cards with metrics
- Interactive filters
- Export buttons
- Responsive chart layouts

## JavaScript Architecture

### Site.js - Core Functionality
```javascript
// API communication
class ApiClient {
    static async get(url) { /* GET requests */ }
    static async post(url, data) { /* POST requests */ }
    static async put(url, data) { /* PUT requests */ }
    static async delete(url) { /* DELETE requests */ }
}

// UI utilities
class UIHelper {
    static showToast(message, type) { /* Toast notifications */ }
    static showModal(modalId) { /* Modal management */ }
    static hideModal(modalId) { /* Modal management */ }
    static showLoading() { /* Loading indicators */ }
    static hideLoading() { /* Loading indicators */ }
}

// Data formatting
class DataFormatter {
    static formatCurrency(amount) { /* Currency formatting */ }
    static formatDate(date) { /* Date formatting */ }
    static formatPhone(phone) { /* Phone formatting */ }
}
```

### Page-Specific Scripts
Each page includes inline JavaScript for:
- Form validation and submission
- Data table management
- Chart initialization and updates
- Event handlers for UI interactions

## API Integration

### Base Configuration
```javascript
const API_BASE_URL = 'https://localhost:7001/api';
const config = {
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
};
```

### CORS Configuration
The API is configured to allow requests from the WebUI:
- Development: `AllowAnyOrigin()` (temporary)
- Production: Specific origin allowlist recommended

### Error Handling
```javascript
async function handleApiResponse(response) {
    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'API request failed');
    }
    return await response.json();
}
```

## Bootstrap 5 Integration

### Layout Structure
```html
<div class="container-fluid">
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
        <!-- Navigation -->
    </nav>
    <main class="py-4">
        @RenderBody()
    </main>
    <footer class="bg-light py-3 mt-auto">
        <!-- Footer -->
    </footer>
</div>
```

### Component Usage
- **Cards**: For data display and forms
- **Modals**: For add/edit operations
- **Tables**: For data listing with sorting
- **Forms**: With validation styling
- **Buttons**: Consistent styling and states
- **Alerts/Toasts**: For user feedback

## Chart.js Integration

### Sales Visualization
```javascript
const chartConfig = {
    type: 'line',
    data: {
        labels: dates,
        datasets: [{
            label: 'Daily Sales',
            data: salesData,
            borderColor: 'rgb(75, 192, 192)',
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            tension: 0.1
        }]
    },
    options: {
        responsive: true,
        plugins: {
            title: {
                display: true,
                text: 'Sales Performance'
            }
        },
        scales: {
            y: {
                beginAtZero: true,
                ticks: {
                    callback: function(value) {
                        return '$' + value.toLocaleString();
                    }
                }
            }
        }
    }
};
```

## Responsive Design

### Breakpoints
- **Mobile**: < 768px (stacked layout)
- **Tablet**: 768px - 992px (condensed layout)
- **Desktop**: > 992px (full layout)

### Mobile Optimizations
- Collapsible navigation
- Stacked form layouts
- Touch-friendly buttons
- Simplified data tables
- Swipe gestures for charts

## Form Validation

### Client-Side Validation
```javascript
function validatePersonnelForm(formData) {
    const errors = [];
    
    if (!formData.name || formData.name.length > 50) {
        errors.push('Name is required and must be 50 characters or less');
    }
    
    if (!formData.age || formData.age < 19) {
        errors.push('Age must be 19 or older');
    }
    
    if (!formData.phone || formData.phone.length > 20) {
        errors.push('Phone is required and must be 20 characters or less');
    }
    
    return errors;
}
```

### Server-Side Integration
- Model validation attributes
- Custom validation rules
- Error message display
- Field highlighting

## IIS Configuration

### Web.config Settings
```xml
<configuration>
  <system.webServer>
    <!-- URL Rewrite for SPA routing -->
    <rewrite>
      <rules>
        <rule name="SPA Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <!-- Exclude static files and Razor Pages -->
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/Sales$" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/Reports$" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/Privacy$" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/Error$" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
    
    <!-- Static content MIME types -->
    <staticContent>
      <remove fileExtension=".css" />
      <mimeMap fileExtension=".css" mimeType="text/css" />
      <remove fileExtension=".js" />
      <mimeMap fileExtension=".js" mimeType="text/javascript" />
    </staticContent>
  </system.webServer>
</configuration>
```

### Deployment Requirements
- **IIS Version**: 10.0+ with URL Rewrite module
- **Application Pool**: No Managed Code, Integrated Pipeline
- **Permissions**: IIS_IUSRS read access to wwwroot
- **HTTPS**: SSL certificate for production

## Authentication Integration (Sprint 4)

### Login Page
```html
<div class="row justify-content-center">
    <div class="col-md-6 col-lg-4">
        <div class="card">
            <div class="card-header">
                <h4>Manager Login</h4>
            </div>
            <div class="card-body">
                <form id="loginForm">
                    <div class="mb-3">
                        <label for="username" class="form-label">Username</label>
                        <input type="text" class="form-control" id="username" required>
                    </div>
                    <div class="mb-3">
                        <label for="password" class="form-label">Password</label>
                        <input type="password" class="form-control" id="password" required>
                    </div>
                    <button type="submit" class="btn btn-primary w-100">Login</button>
                </form>
            </div>
        </div>
    </div>
</div>
```

### Session Management
- ASP.NET Core authentication cookies
- Session timeout handling
- Automatic redirects for unauthenticated users
- User context display in navigation

## Performance Optimization

### Client-Side Caching
- Browser caching for static assets
- Local storage for user preferences
- Session storage for temporary data
- API response caching where appropriate

### Bundle Optimization
```html
<!-- Development -->
<link href="~/css/bootstrap.min.css" rel="stylesheet" />
<link href="~/css/site.css" rel="stylesheet" />

<!-- Production (bundled) -->
<link href="~/css/bundle.min.css" rel="stylesheet" />
```

### Lazy Loading
- Chart.js loaded only when needed
- Modal content loaded on demand
- Large datasets paginated
- Images lazy loaded

## Accessibility

### WCAG Compliance
- Semantic HTML structure
- ARIA labels and roles
- Keyboard navigation support
- Screen reader compatibility
- Color contrast compliance

### Features
- Focus management in modals
- Skip navigation links
- Alt text for images
- Form label associations
- Error message announcements

## Browser Compatibility

### Supported Browsers
- **Chrome**: 90+
- **Firefox**: 88+
- **Safari**: 14+
- **Edge**: 90+
- **Internet Explorer**: Not supported

### Progressive Enhancement
- Core functionality without JavaScript
- Graceful degradation for older browsers
- Feature detection over browser detection
- Polyfills for missing features

## Development

### Local Development
```bash
# Restore packages
dotnet restore

# Run application
dotnet run

# Watch for changes
dotnet watch run
```

### Hot Reload
- CSS changes applied immediately
- JavaScript changes require refresh
- Razor page changes applied automatically
- Static file changes reflected instantly

## Testing

### Manual Testing Checklist
- [ ] All forms validate correctly
- [ ] CRUD operations work properly
- [ ] Charts render and update
- [ ] Responsive design functions
- [ ] Cross-browser compatibility
- [ ] Accessibility compliance

### Automated Testing
- Unit tests for JavaScript functions
- Integration tests for API calls
- UI tests with Selenium
- Performance testing with Lighthouse

## Security

### Client-Side Security
- Input sanitization
- XSS prevention
- CSRF protection
- Secure cookie settings
- Content Security Policy headers

### Data Protection
- Sensitive data not logged
- API keys not exposed
- User input validation
- Secure communication (HTTPS)

## Troubleshooting

### Common Issues
- **Static files not loading**: Check MIME types in web.config
- **API calls failing**: Verify CORS configuration
- **Charts not rendering**: Ensure Chart.js is loaded
- **Navigation not working**: Check URL rewrite rules
- **Forms not submitting**: Validate JavaScript and API endpoints

### Debug Tools
- Browser developer tools
- Network tab for API calls
- Console for JavaScript errors
- Application tab for storage
- Lighthouse for performance

## Future Enhancements

### Planned Features
- Real-time notifications
- Offline capability
- Mobile app companion
- Advanced reporting dashboards
- Multi-language support

### Technical Improvements
- TypeScript migration
- Modern JavaScript frameworks
- Progressive Web App features
- Advanced caching strategies
- Micro-frontend architecture