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
        var self = this;
        
        // View toggle buttons
        $('#gridViewBtn').on('click', function() { self.switchToGridView(); });
        $('#chartViewBtn').on('click', function() { self.switchToChartView(); });
        
        // Load sales button
        $('#loadSalesBtn').on('click', function() { self.loadSalesData(); });
        
        // Personnel selection change
        $('#personnelSelect').on('change', function() {
            var value = $(this).val();
            self.currentPersonnelId = value ? parseInt(value) : null;
            if (self.currentPersonnelId) {
                self.loadSalesData();
            }
        });

        // Add sales form
        $('#addSalesForm').on('submit', function(e) { self.handleAddSales(e); });
    }

    setDefaultDateRange() {
        var now = new Date();
        var firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
        var lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        
        $('#dateFrom').val(firstDay.toISOString().split('T')[0]);
        $('#dateTo').val(lastDay.toISOString().split('T')[0]);
        $('#salesDate').val(now.toISOString().split('T')[0]);
    }

    loadPersonnelData() {
        var self = this;
        console.log('Loading personnel data...');
        
        apiClient.getAllPersonnel(function(error, response) {
            console.log('Personnel API response:', response);
            
            if (error) {
                console.error('Error loading personnel:', error);
                self.showError('Error loading personnel: ' + error.message);
            } else if (response.success) {
                self.personnelData = response.data;
                console.log('Personnel data loaded:', self.personnelData);
                self.populatePersonnelSelects();
            } else {
                self.showError('Failed to load personnel data: ' + response.message);
            }
        });
    }

    populatePersonnelSelects() {
        console.log('Populating personnel selects...');
        var $personnelSelect = $('#personnelSelect');
        var $salesPersonnelSelect = $('#salesPersonnelId');
        
        if ($personnelSelect.length === 0 || $salesPersonnelSelect.length === 0) {
            console.error('Personnel select elements not found');
            return;
        }
        
        // Clear existing options
        $personnelSelect.html('<option value="">Select Personnel</option>');
        $salesPersonnelSelect.html('<option value="">Select Personnel</option>');
        
        // Add personnel options
        if (this.personnelData && this.personnelData.length > 0) {
            var self = this;
            $.each(this.personnelData, function(index, person) {
                var optionText = person.name + ' (ID: ' + person.id + ')';
                $personnelSelect.append($('<option></option>').val(person.id).text(optionText));
                $salesPersonnelSelect.append($('<option></option>').val(person.id).text(optionText));
            });
            console.log('Added ' + this.personnelData.length + ' personnel options');
        } else {
            console.warn('No personnel data available to populate');
        }
    }

    async loadSalesData() {
        var personnelId = this.currentPersonnelId;
        var fromDate = $('#dateFrom').val();
        var toDate = $('#dateTo').val();

        if (!personnelId) {
            this.showError('Please select personnel first');
            return;
        }

        this.showLoading(true);

        try {
            var queryParams = [];
            queryParams.push('personnelId=' + encodeURIComponent(personnelId));
            if (fromDate) queryParams.push('from=' + encodeURIComponent(fromDate));
            if (toDate) queryParams.push('to=' + encodeURIComponent(toDate));

            var url = API_CONFIG.baseUrl + '/sales?' + queryParams.join('&');
            
            var self = this;
            $.ajax({
                url: url,
                method: 'GET',
                success: function(result) {
                    if (result.success) {
                        self.currentSalesData = result.data;
                        self.updateSalesDisplay();
                    } else {
                        self.showError('Failed to load sales data: ' + result.message);
                    }
                },
                error: function(xhr, status, error) {
                    self.showError('Error loading sales: ' + error);
                },
                complete: function() {
                    self.showLoading(false);
                }
            });
        } catch (error) {
            this.showError('Error loading sales: ' + error.message);
            this.showLoading(false);
        }
    }

    updateSalesDisplay() {
        $('#salesCount').text(this.currentSalesData.length + ' records');
        
        if (this.currentView === 'grid') {
            this.renderSalesGrid();
        } else {
            this.renderSalesChart();
        }
    }

    renderSalesGrid() {
        var $tbody = $('#salesTableBody');
        
        if (this.currentSalesData.length === 0) {
            $tbody.html(
                '<tr>' +
                    '<td colspan="5" class="text-center text-muted">' +
                        '<i class="bi bi-info-circle"></i> No sales records found for the selected criteria' +
                    '</td>' +
                '</tr>'
            );
            return;
        }

        var html = '';
        var self = this;
        $.each(this.currentSalesData, function(index, sale) {
            var personnel = null;
            $.each(self.personnelData, function(i, p) {
                if (p.id === sale.personnelId) {
                    personnel = p;
                    return false; // break
                }
            });
            
            var personnelName = personnel ? personnel.name : 'ID: ' + sale.personnelId;
            var formattedDate = new Date(sale.reportDate).toLocaleDateString();
            var formattedAmount = '$' + parseFloat(sale.salesAmount).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });

            html += '<tr>' +
                '<td>' + sale.id + '</td>' +
                '<td>' + personnelName + '</td>' +
                '<td>' + formattedDate + '</td>' +
                '<td>' + formattedAmount + '</td>' +
                '<td>' +
                    '<button class="btn btn-sm btn-outline-danger" onclick="salesManager.deleteSales(' + sale.id + ')">' +
                        '<i class="bi bi-trash"></i> Delete' +
                    '</button>' +
                '</td>' +
            '</tr>';
        });
        
        $tbody.html(html);
    }

    renderSalesChart() {
        var self = this;
        
        // Check for Chart.js with timeout
        var checkChart = function(attempts) {
            if (typeof Chart !== 'undefined' || typeof window.Chart !== 'undefined') {
                self.createChart();
            } else if (attempts < 50) {
                setTimeout(function() {
                    checkChart(attempts + 1);
                }, 100);
            } else {
                self.showError('Chart.js failed to load after 5 seconds');
            }
        };
        
        checkChart(0);
    }
    
    createChart() {
        var ChartConstructor = Chart || window.Chart;
        if (!ChartConstructor) {
            this.showError('Chart.js is not available');
            return;
        }

        var canvas = document.getElementById('salesChart');
        if (!canvas) {
            this.showError('Chart canvas not found');
            return;
        }
        
        var ctx = canvas.getContext('2d');
        
        // Destroy existing chart
        if (this.salesChart) {
            this.salesChart.destroy();
        }

        // Group sales by date
        var salesByDate = {};
        $.each(this.currentSalesData, function(index, sale) {
            var date = new Date(sale.reportDate).toLocaleDateString();
            salesByDate[date] = (salesByDate[date] || 0) + sale.salesAmount;
        });

        var dates = Object.keys(salesByDate).sort();
        var amounts = [];
        $.each(dates, function(index, date) {
            amounts.push(salesByDate[date]);
        });

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
                                return '$' + parseFloat(value).toLocaleString('en-US', {
                                    minimumFractionDigits: 2,
                                    maximumFractionDigits: 2
                                });
                            }
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return 'Sales: $' + parseFloat(context.parsed.y).toLocaleString('en-US', {
                                    minimumFractionDigits: 2,
                                    maximumFractionDigits: 2
                                });
                            }
                        }
                    }
                }
            }
        });
    }

    switchToGridView() {
        this.currentView = 'grid';
        $('#gridViewBtn').addClass('active');
        $('#chartViewBtn').removeClass('active');
        $('#gridView').show();
        $('#chartView').hide();
        
        if (this.currentSalesData.length > 0) {
            this.renderSalesGrid();
        }
    }

    switchToChartView() {
        var self = this;
        this.currentView = 'chart';
        $('#chartViewBtn').addClass('active');
        $('#gridViewBtn').removeClass('active');
        $('#gridView').hide();
        $('#chartView').show();
        
        if (this.currentSalesData.length > 0) {
            setTimeout(function() {
                self.renderSalesChart();
            }, 100); // Allow DOM to update
        }
    }

    handleAddSales(event) {
        event.preventDefault();
        
        var personnelId = parseInt($('#salesPersonnelId').val());
        var salesDate = $('#salesDate').val();
        var salesAmount = parseFloat($('#salesAmount').val());

        if (!personnelId || !salesDate || salesAmount < 0) {
            this.showError('Please fill in all required fields with valid values');
            return;
        }

        // Check if date is in the future
        var selectedDate = new Date(salesDate);
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        
        if (selectedDate > today) {
            this.showError('Sales date cannot be in the future');
            return;
        }

        var salesData = {
            personnelId: personnelId,
            reportDate: salesDate,
            salesAmount: salesAmount
        };

        var self = this;
        $.ajax({
            url: API_CONFIG.baseUrl + '/sales',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(salesData),
            success: function(result) {
                if (result.success) {
                    self.showSuccess('Sales record added successfully');
                    $('#addSalesForm')[0].reset();
                    $('#salesDate').val(new Date().toISOString().split('T')[0]);
                    
                    // Reload sales data if current personnel matches
                    if (self.currentPersonnelId === personnelId) {
                        self.loadSalesData();
                    }
                } else {
                    self.showError('Failed to add sales record: ' + (result.message || 'Unknown error'));
                    if (result.errors && result.errors.length > 0) {
                        $.each(result.errors, function(index, error) {
                            self.showError(error);
                        });
                    }
                }
            },
            error: function(xhr, status, error) {
                self.showError('Error adding sales record: ' + error);
            }
        });
    }

    deleteSales(salesId) {
        if (!confirm('Are you sure you want to delete this sales record?')) {
            return;
        }

        var self = this;
        $.ajax({
            url: API_CONFIG.baseUrl + '/sales/' + salesId,
            method: 'DELETE',
            success: function(result) {
                if (result.success) {
                    self.showSuccess('Sales record deleted successfully');
                    self.loadSalesData(); // Refresh the data
                } else {
                    self.showError('Failed to delete sales record: ' + result.message);
                }
            },
            error: function(xhr, status, error) {
                self.showError('Error deleting sales record: ' + error);
            }
        });
    }

    showLoading(show) {
        if (show) {
            $('#loadingIndicator').show();
        } else {
            $('#loadingIndicator').hide();
        }
    }

    showError(message) {
        // Create and show error alert
        var alertHtml = '<div class="alert alert-danger alert-dismissible fade show">' +
            '<i class="bi bi-exclamation-triangle"></i> ' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';
        
        var $alert = $(alertHtml);
        $('.container-fluid').prepend($alert);
        
        // Auto-dismiss after 5 seconds
        setTimeout(function() {
            $alert.fadeOut(function() {
                $alert.remove();
            });
        }, 5000);
    }

    showSuccess(message) {
        // Create and show success alert
        var alertHtml = '<div class="alert alert-success alert-dismissible fade show">' +
            '<i class="bi bi-check-circle"></i> ' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';
        
        var $alert = $(alertHtml);
        $('.container-fluid').prepend($alert);
        
        // Auto-dismiss after 3 seconds
        setTimeout(function() {
            $alert.fadeOut(function() {
                $alert.remove();
            });
        }, 3000);
    }
}

// Initialize the sales manager when the page loads
var salesManager;
$(document).ready(function() {
    // Initialize immediately - Chart.js will be checked when needed
    salesManager = new SalesManager();
});
