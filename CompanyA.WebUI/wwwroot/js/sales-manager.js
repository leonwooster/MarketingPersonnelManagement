// Sales Management JavaScript Module
class SalesManager {
    constructor() {
        this.currentPersonnelId = null;
        this.currentSalesData = [];
        this.personnelData = [];
        this.salesChart = null;
        this.currentView = 'grid';
        
        this.initializeEventListeners();
        this.loadPersonnelData();
        this.setDefaultDateRange();
    }

    initializeEventListeners() {
        // View toggle buttons
        document.getElementById('gridViewBtn').addEventListener('click', () => this.switchToGridView());
        document.getElementById('chartViewBtn').addEventListener('click', () => this.switchToChartView());
        
        // Load sales button
        document.getElementById('loadSalesBtn').addEventListener('click', () => this.loadSalesData());
        
        // Personnel selection change
        document.getElementById('personnelSelect').addEventListener('change', (e) => {
            this.currentPersonnelId = e.target.value ? parseInt(e.target.value) : null;
            if (this.currentPersonnelId) {
                this.loadSalesData();
            }
        });

        // Add sales form
        document.getElementById('addSalesForm').addEventListener('submit', (e) => this.handleAddSales(e));
    }

    setDefaultDateRange() {
        const now = new Date();
        const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
        const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        
        document.getElementById('dateFrom').value = firstDay.toISOString().split('T')[0];
        document.getElementById('dateTo').value = lastDay.toISOString().split('T')[0];
        document.getElementById('salesDate').value = now.toISOString().split('T')[0];
    }

    async loadPersonnelData() {
        try {
            console.log('Loading personnel data...');
            const response = await apiClient.getAllPersonnel();
            console.log('Personnel API response:', response);
            
            if (response.success) {
                this.personnelData = response.data;
                console.log('Personnel data loaded:', this.personnelData);
                this.populatePersonnelSelects();
            } else {
                this.showError('Failed to load personnel data: ' + response.message);
            }
        } catch (error) {
            console.error('Error loading personnel:', error);
            this.showError('Error loading personnel: ' + error.message);
        }
    }

    populatePersonnelSelects() {
        console.log('Populating personnel selects...');
        const personnelSelect = document.getElementById('personnelSelect');
        const salesPersonnelSelect = document.getElementById('salesPersonnelId');
        
        if (!personnelSelect || !salesPersonnelSelect) {
            console.error('Personnel select elements not found');
            return;
        }
        
        // Clear existing options
        personnelSelect.innerHTML = '<option value="">Select Personnel</option>';
        salesPersonnelSelect.innerHTML = '<option value="">Select Personnel</option>';
        
        // Add personnel options
        if (this.personnelData && this.personnelData.length > 0) {
            this.personnelData.forEach(person => {
                const option1 = new Option(`${person.name} (ID: ${person.id})`, person.id);
                const option2 = new Option(`${person.name} (ID: ${person.id})`, person.id);
                personnelSelect.appendChild(option1);
                salesPersonnelSelect.appendChild(option2);
            });
            console.log(`Added ${this.personnelData.length} personnel options`);
        } else {
            console.warn('No personnel data available to populate');
        }
    }

    async loadSalesData() {
        const personnelId = this.currentPersonnelId;
        const fromDate = document.getElementById('dateFrom').value;
        const toDate = document.getElementById('dateTo').value;

        if (!personnelId) {
            this.showError('Please select personnel first');
            return;
        }

        this.showLoading(true);

        try {
            const queryParams = new URLSearchParams();
            queryParams.append('personnelId', personnelId);
            if (fromDate) queryParams.append('from', fromDate);
            if (toDate) queryParams.append('to', toDate);

            const response = await fetch(`${API_CONFIG.baseUrl}/sales?${queryParams}`);
            const result = await response.json();

            if (result.success) {
                this.currentSalesData = result.data;
                await this.updateSalesDisplay();
            } else {
                this.showError('Failed to load sales data: ' + result.message);
            }
        } catch (error) {
            this.showError('Error loading sales: ' + error.message);
        } finally {
            this.showLoading(false);
        }
    }

    async updateSalesDisplay() {
        document.getElementById('salesCount').textContent = `${this.currentSalesData.length} records`;
        
        if (this.currentView === 'grid') {
            this.renderSalesGrid();
        } else {
            await this.renderSalesChart();
        }
    }

    renderSalesGrid() {
        const tbody = document.getElementById('salesTableBody');
        
        if (this.currentSalesData.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center text-muted">
                        <i class="bi bi-info-circle"></i> No sales records found for the selected criteria
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = this.currentSalesData.map(sale => {
            const personnel = this.personnelData.find(p => p.id === sale.personnelId);
            const personnelName = personnel ? personnel.name : `ID: ${sale.personnelId}`;
            const formattedDate = new Date(sale.reportDate).toLocaleDateString();
            const formattedAmount = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            }).format(sale.salesAmount);

            return `
                <tr>
                    <td>${sale.id}</td>
                    <td>${personnelName}</td>
                    <td>${formattedDate}</td>
                    <td>${formattedAmount}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-danger" onclick="salesManager.deleteSales(${sale.id})">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    }

    async renderSalesChart() {
        // Debug what's actually available
        console.log('Available globals:', Object.keys(window).filter(k => k.toLowerCase().includes('chart')));
        console.log('Chart object:', typeof Chart);
        console.log('window.Chart:', typeof window.Chart);
        
        // Wait for Chart.js to be available with timeout
        let attempts = 0;
        while (typeof Chart === 'undefined' && typeof window.Chart === 'undefined' && attempts < 50) {
            console.log('Waiting for Chart.js...', attempts);
            await new Promise(resolve => setTimeout(resolve, 100));
            attempts++;
        }
        
        const ChartConstructor = Chart || window.Chart;
        if (!ChartConstructor) {
            this.showError('Chart.js failed to load after 5 seconds');
            return;
        }

        const ctx = document.getElementById('salesChart').getContext('2d');
        
        // Destroy existing chart
        if (this.salesChart) {
            this.salesChart.destroy();
        }

        // Group sales by date
        const salesByDate = {};
        this.currentSalesData.forEach(sale => {
            const date = new Date(sale.reportDate).toLocaleDateString();
            salesByDate[date] = (salesByDate[date] || 0) + sale.salesAmount;
        });

        const dates = Object.keys(salesByDate).sort();
        const amounts = dates.map(date => salesByDate[date]);

        this.salesChart = new ChartConstructor(ctx, {
            type: 'bar',
            data: {
                labels: dates,
                datasets: [{
                    label: 'Sales Amount',
                    data: amounts,
                    backgroundColor: 'rgba(13, 110, 253, 0.8)',
                    borderColor: 'rgba(13, 110, 253, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return new Intl.NumberFormat('en-US', {
                                    style: 'currency',
                                    currency: 'USD'
                                }).format(value);
                            }
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return 'Sales: ' + new Intl.NumberFormat('en-US', {
                                    style: 'currency',
                                    currency: 'USD'
                                }).format(context.parsed.y);
                            }
                        }
                    }
                }
            }
        });
    }

    switchToGridView() {
        this.currentView = 'grid';
        document.getElementById('gridViewBtn').classList.add('active');
        document.getElementById('chartViewBtn').classList.remove('active');
        document.getElementById('gridView').style.display = 'block';
        document.getElementById('chartView').style.display = 'none';
        
        if (this.currentSalesData.length > 0) {
            this.renderSalesGrid();
        }
    }

    switchToChartView() {
        this.currentView = 'chart';
        document.getElementById('chartViewBtn').classList.add('active');
        document.getElementById('gridViewBtn').classList.remove('active');
        document.getElementById('gridView').style.display = 'none';
        document.getElementById('chartView').style.display = 'block';
        
        if (this.currentSalesData.length > 0) {
            setTimeout(async () => await this.renderSalesChart(), 100); // Allow DOM to update
        }
    }

    async handleAddSales(event) {
        event.preventDefault();
        
        const personnelId = parseInt(document.getElementById('salesPersonnelId').value);
        const salesDate = document.getElementById('salesDate').value;
        const salesAmount = parseFloat(document.getElementById('salesAmount').value);

        if (!personnelId || !salesDate || salesAmount < 0) {
            this.showError('Please fill in all required fields with valid values');
            return;
        }

        // Check if date is in the future
        const selectedDate = new Date(salesDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        if (selectedDate > today) {
            this.showError('Sales date cannot be in the future');
            return;
        }

        try {
            const salesData = {
                personnelId: personnelId,
                reportDate: salesDate,
                salesAmount: salesAmount
            };

            const response = await fetch(`${API_CONFIG.baseUrl}/sales`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(salesData)
            });

            const result = await response.json();

            if (result.success) {
                this.showSuccess('Sales record added successfully');
                document.getElementById('addSalesForm').reset();
                document.getElementById('salesDate').value = new Date().toISOString().split('T')[0];
                
                // Reload sales data if current personnel matches
                if (this.currentPersonnelId === personnelId) {
                    await this.loadSalesData();
                }
            } else {
                this.showError('Failed to add sales record: ' + (result.message || 'Unknown error'));
                if (result.errors && result.errors.length > 0) {
                    result.errors.forEach(error => this.showError(error));
                }
            }
        } catch (error) {
            this.showError('Error adding sales record: ' + error.message);
        }
    }

    async deleteSales(salesId) {
        if (!confirm('Are you sure you want to delete this sales record?')) {
            return;
        }

        try {
            const response = await fetch(`${API_CONFIG.baseUrl}/sales/${salesId}`, {
                method: 'DELETE'
            });

            const result = await response.json();

            if (result.success) {
                this.showSuccess('Sales record deleted successfully');
                await this.loadSalesData(); // Refresh the data
            } else {
                this.showError('Failed to delete sales record: ' + result.message);
            }
        } catch (error) {
            this.showError('Error deleting sales record: ' + error.message);
        }
    }

    showLoading(show) {
        document.getElementById('loadingIndicator').style.display = show ? 'block' : 'none';
    }

    showError(message) {
        // Create and show error alert
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger alert-dismissible fade show';
        alertDiv.innerHTML = `
            <i class="bi bi-exclamation-triangle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        const container = document.querySelector('.container-fluid');
        container.insertBefore(alertDiv, container.firstChild);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 5000);
    }

    showSuccess(message) {
        // Create and show success alert
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-success alert-dismissible fade show';
        alertDiv.innerHTML = `
            <i class="bi bi-check-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        const container = document.querySelector('.container-fluid');
        container.insertBefore(alertDiv, container.firstChild);
        
        // Auto-dismiss after 3 seconds
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 3000);
    }
}

// Initialize the sales manager when the page loads
let salesManager;
document.addEventListener('DOMContentLoaded', function() {
    // Initialize immediately - Chart.js will be checked when needed
    salesManager = new SalesManager();
});
