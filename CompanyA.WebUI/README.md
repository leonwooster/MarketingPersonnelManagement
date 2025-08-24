# CompanyA.WebUI

Single Page Application (SPA) for the Marketing Personnel Management System.

## Overview

This project hosts the frontend user interface for managing marketing personnel, viewing sales data, and generating reports. Built as a responsive SPA using Bootstrap and vanilla JavaScript.

## Technology Stack

- **Framework**: ASP.NET Core MVC/Razor Pages (hosting only)
- **Frontend**: HTML5, CSS3, JavaScript (ES6+)
- **UI Library**: Bootstrap 5
- **Charts**: Chart.js
- **HTTP Client**: Fetch API
- **Icons**: Bootstrap Icons

## Features

### Personnel Management
- Grid view with sorting and basic search
- Add/Edit personnel via modal forms
- Delete with confirmation dialog
- Real-time validation feedback
- Responsive design for mobile/tablet

### Sales Visualization
- Monthly sales view for selected personnel
- Toggle between grid and graph display
- Add/Delete sales records (no editing)
- Interactive charts with Chart.js
- Date range filtering

### Reporting
- Management overview reports
- Commission payout calculations
- Export to CSV functionality
- Printable report layouts

## File Structure

```
CompanyA.WebUI/
├── wwwroot/
│   ├── css/
│   │   ├── bootstrap.min.css
│   │   ├── site.css
│   │   └── personnel.css
│   ├── js/
│   │   ├── bootstrap.bundle.min.js
│   │   ├── chart.min.js
│   │   ├── app.js
│   │   ├── personnel.js
│   │   ├── sales.js
│   │   ├── reports.js
│   │   └── api-client.js
│   └── lib/
├── Views/
│   ├── Home/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
└── Controllers/
    └── HomeController.cs
```

## API Integration

### Base Configuration
```javascript
const API_BASE_URL = 'http://localhost:5001/api';
```

### HTTP Client Usage
```javascript
// GET request
const personnel = await apiClient.get('/personnel');

// POST request with validation
const result = await apiClient.post('/personnel', personnelData);
if (result.success) {
    // Handle success
} else {
    // Display validation errors
}
```

## UI Components

### Personnel Grid
- Sortable columns
- Pagination for large datasets
- Search/filter functionality
- Action buttons (Edit, Delete)
- Responsive table design

### Modal Forms
- Add/Edit personnel forms
- Client-side validation
- Server error display
- Bootstrap modal integration

### Sales Panel
- Personnel selection dropdown
- Month/year picker
- Grid/Graph toggle buttons
- Add sales form
- Delete confirmation

### Charts
- Monthly sales bar chart
- Daily sales line chart
- Top performers pie chart
- Responsive chart sizing

## Validation

### Client-Side Rules
- Name: Required, max 50 characters
- Age: Required, numeric, >= 19
- Phone: Required, max 20 characters
- Commission Profile: Required selection
- Bank details: Optional, max 20 characters each

### Error Display
```javascript
function showValidationErrors(errors) {
    errors.forEach(error => {
        const field = document.getElementById(error.field);
        field.classList.add('is-invalid');
        const feedback = field.nextElementSibling;
        feedback.textContent = error.message;
    });
}
```

## Responsive Design

### Breakpoints
- **Mobile**: < 768px (stacked layout)
- **Tablet**: 768px - 1024px (condensed grid)
- **Desktop**: > 1024px (full layout)

### Mobile Optimizations
- Touch-friendly buttons
- Collapsible navigation
- Swipeable charts
- Optimized modal sizes

## Browser Support

### Tested Browsers
- Chrome 90+
- Firefox 88+
- Edge 90+
- Safari 14+ (basic support)

### Required Features
- ES6+ JavaScript support
- CSS Grid and Flexbox
- Fetch API
- Local Storage

## Configuration

### Development Settings
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001/api",
    "Timeout": 30000
  },
  "UI": {
    "PageSize": 10,
    "ChartColors": ["#007bff", "#28a745", "#ffc107"]
  }
}
```

## Development

### Running Locally
```bash
# Restore packages
dotnet restore

# Run application
dotnet run

# Navigate to https://localhost:5000
```

### Building Assets
```bash
# No build process required for vanilla JS
# Static files served directly from wwwroot
```

### Testing
- Manual testing across browsers
- Responsive design testing
- API integration testing
- Form validation testing

## Deployment

### Build for Production
```bash
dotnet publish -c Release -o ../Deploy/WebUI
```

### Static File Optimization
- Minified CSS/JS files
- Compressed images
- CDN integration for libraries
- Cache headers configuration

## Performance

### Optimization Techniques
- Lazy loading of charts
- Debounced search inputs
- Efficient DOM updates
- Minimal HTTP requests

### Caching Strategy
- Browser cache for static assets
- Local storage for user preferences
- Session storage for temporary data

## Security

### Client-Side Protection
- Input sanitization
- XSS prevention via proper encoding
- CSRF token integration (if auth added)
- Secure API communication

### Data Validation
- Client-side validation for UX
- Server-side validation as primary defense
- Proper error message handling

## Accessibility

### WCAG Compliance
- Semantic HTML structure
- ARIA labels for interactive elements
- Keyboard navigation support
- High contrast color scheme
- Screen reader compatibility

## Troubleshooting

### Common Issues
- **API Connection**: Check base URL configuration
- **CORS Errors**: Verify API CORS settings
- **Chart Not Loading**: Check Chart.js library inclusion
- **Modal Issues**: Verify Bootstrap JS inclusion

### Debug Tools
- Browser developer tools
- Network tab for API calls
- Console for JavaScript errors
- Responsive design mode

## Future Enhancements

### Planned Features
- Real-time updates via SignalR
- Advanced filtering and search
- Bulk operations
- Print-friendly layouts
- Offline capability

### Technical Improvements
- TypeScript migration
- Modern build pipeline
- Component-based architecture
- Automated testing suite
