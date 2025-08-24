// Reports Management JavaScript Module
function ReportsManager() {
    this.currentReportType = 'management';
    this.personnelData = [];
    this.currentReportData = null;
    this.init();
}

ReportsManager.prototype.init = function() {
    this.setupEventListeners();
    this.initializeFilters();
    this.loadPersonnelData();
};

ReportsManager.prototype.setupEventListeners = function() {
    var self = this;
    
    // Report type toggle buttons
    $('#managementReportBtn').on('click', function() {
        self.switchToManagementReport();
    });
    
    $('#commissionReportBtn').on('click', function() {
        self.switchToCommissionReport();
    });
    
    // Generate report button
    $('#generateReportBtn').on('click', function() {
        self.generateReport();
    });
    
    // CSV download buttons
    $('#downloadManagementCsv').on('click', function() {
        self.downloadCsv('management-overview');
    });
    
    $('#downloadCommissionCsv').on('click', function() {
        self.downloadCsv('commission-payout');
    });
};

ReportsManager.prototype.initializeFilters = function() {
    // Populate year dropdown
    var currentYear = new Date().getFullYear();
    var $yearSelect = $('#reportYear');
    
    for (var year = currentYear; year >= currentYear - 5; year--) {
        $yearSelect.append($('<option></option>').val(year).text(year));
    }
    
    // Set current month and year as default
    var currentMonth = new Date().getMonth() + 1;
    $('#reportYear').val(currentYear);
    $('#reportMonth').val(currentMonth);
};

ReportsManager.prototype.loadPersonnelData = function() {
    var self = this;
    
    apiClient.getAllPersonnel(function(error, response) {
        if (error) {
            console.error('Error loading personnel:', error);
        } else if (response.success) {
            self.personnelData = response.data;
            self.populatePersonnelFilter();
        } else {
            console.error('Failed to load personnel:', response.message);
        }
    });
};

ReportsManager.prototype.populatePersonnelFilter = function() {
    var $personnelSelect = $('#reportPersonnel');
    $personnelSelect.html('<option value="">All Personnel</option>');
    
    $.each(this.personnelData, function(index, person) {
        var optionText = person.name + ' (ID: ' + person.id + ')';
        $personnelSelect.append($('<option></option>').val(person.id).text(optionText));
    });
};

ReportsManager.prototype.switchToManagementReport = function() {
    this.currentReportType = 'management';
    $('#managementReportBtn').addClass('active');
    $('#commissionReportBtn').removeClass('active');
    $('#managementReport').show();
    $('#commissionReport').hide();
};

ReportsManager.prototype.switchToCommissionReport = function() {
    this.currentReportType = 'commission';
    $('#commissionReportBtn').addClass('active');
    $('#managementReportBtn').removeClass('active');
    $('#commissionReport').show();
    $('#managementReport').hide();
};

ReportsManager.prototype.generateReport = function() {
    var year = parseInt($('#reportYear').val());
    var month = parseInt($('#reportMonth').val());
    var personnelId = $('#reportPersonnel').val();
    
    if (!year || !month) {
        this.showError('Please select year and month');
        return;
    }
    
    this.showLoading(true);
    
    if (this.currentReportType === 'management') {
        this.generateManagementReport(year, month, personnelId);
    } else {
        this.generateCommissionReport(year, month, personnelId);
    }
};

ReportsManager.prototype.generateManagementReport = function(year, month, personnelId) {
    var self = this;
    var url = API_CONFIG.baseUrl + '/reports/management-overview?year=' + year + '&month=' + month;
    if (personnelId) {
        url += '&personnelId=' + personnelId;
    }
    
    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'json',
        success: function(response) {
            console.log('Management report response:', response);
            if (response && response.success && response.data) {
                self.currentReportData = response.data;
                self.renderManagementReport(response.data);
            } else {
                self.showError('Failed to generate report: ' + (response ? response.message : 'Invalid response'));
            }
        },
        error: function(xhr, status, error) {
            console.error('AJAX Error:', xhr, status, error);
            self.showError('Error generating report: ' + error);
        },
        complete: function() {
            self.showLoading(false);
        }
    });
};

ReportsManager.prototype.generateCommissionReport = function(year, month, personnelId) {
    var self = this;
    var url = API_CONFIG.baseUrl + '/reports/commission-payout?year=' + year + '&month=' + month;
    if (personnelId) {
        url += '&personnelId=' + personnelId;
    }
    
    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'json',
        success: function(response) {
            console.log('Commission report response:', response);
            if (response && response.success && response.data) {
                self.currentReportData = response.data;
                self.renderCommissionReport(response.data);
            } else {
                self.showError('Failed to generate report: ' + (response ? response.message : 'Invalid response'));
            }
        },
        error: function(xhr, status, error) {
            console.error('AJAX Error:', xhr, status, error);
            self.showError('Error generating report: ' + error);
        },
        complete: function() {
            self.showLoading(false);
        }
    });
};

ReportsManager.prototype.renderManagementReport = function(data) {
    if (!data || !data.summary) {
        console.error('Invalid data passed to renderManagementReport:', data);
        this.showError('Invalid report data received');
        return;
    }
    
    // Update summary cards
    $('#totalSales').text(this.formatCurrency(data.summary.totalSales || 0));
    $('#totalTransactions').text((data.summary.totalTransactions || 0).toLocaleString());
    $('#averagePerPerson').text(this.formatCurrency(data.summary.averagePerPerson || 0));
    $('#noSalesDays').text((data.summary.daysWithNoSales || 0) + ' / ' + (data.summary.daysInMonth || 0));
    
    // Update top performers table
    var $tbody = $('#topPerformersTable');
    $tbody.empty();
    
    if (data.topPerformers && data.topPerformers.length > 0) {
        var self = this;
        $.each(data.topPerformers, function(index, performer) {
            var rank = index + 1;
            var rankIcon = '';
            if (rank === 1) rankIcon = '<i class="bi bi-trophy-fill text-warning"></i> ';
            else if (rank === 2) rankIcon = '<i class="bi bi-award-fill text-secondary"></i> ';
            else if (rank === 3) rankIcon = '<i class="bi bi-award-fill text-warning"></i> ';
            
            var row = '<tr>' +
                '<td>' + rankIcon + rank + '</td>' +
                '<td>' + performer.personnelName + '</td>' +
                '<td>' + self.formatCurrency(performer.totalSales) + '</td>' +
                '<td>' + performer.transactionCount + '</td>' +
            '</tr>';
            $tbody.append(row);
        });
    } else {
        $tbody.html('<tr><td colspan="4" class="text-center text-muted">No sales data found for this period</td></tr>');
    }
};

ReportsManager.prototype.renderCommissionReport = function(data) {
    if (!data || !data.summary) {
        console.error('Invalid data passed to renderCommissionReport:', data);
        this.showError('Invalid commission report data received');
        return;
    }
    
    // Update summary cards
    $('#commissionTotalSales').text(this.formatCurrency(data.summary.totalSales || 0));
    $('#totalFixedCommissions').text(this.formatCurrency(data.summary.totalFixedCommissions || 0));
    $('#totalVariableCommissions').text(this.formatCurrency(data.summary.totalVariableCommissions || 0));
    $('#totalPayout').text(this.formatCurrency(data.summary.totalPayout || 0));
    
    // Update personnel payouts table
    var $tbody = $('#personnelPayoutsTable');
    $tbody.empty();
    
    if (data.personnelPayouts && data.personnelPayouts.length > 0) {
        var self = this;
        $.each(data.personnelPayouts, function(index, payout) {
            var row = '<tr>' +
                '<td>' + payout.personnelName + '</td>' +
                '<td>' + self.formatCurrency(payout.monthlySales) + '</td>' +
                '<td>' + self.formatCurrency(payout.commissionFixed) + '</td>' +
                '<td>' + self.formatPercentage(payout.commissionPercentage) + '</td>' +
                '<td>' + self.formatCurrency(payout.commissionVariable) + '</td>' +
                '<td><strong>' + self.formatCurrency(payout.totalPayout) + '</strong></td>' +
            '</tr>';
            $tbody.append(row);
        });
    } else {
        $tbody.html('<tr><td colspan="6" class="text-center text-muted">No commission data found for this period</td></tr>');
    }
};

ReportsManager.prototype.downloadCsv = function(reportType) {
    var year = parseInt($('#reportYear').val());
    var month = parseInt($('#reportMonth').val());
    var personnelId = $('#reportPersonnel').val();
    
    if (!year || !month) {
        this.showError('Please generate a report first');
        return;
    }
    
    var url = API_CONFIG.baseUrl + '/reports/' + reportType + '?year=' + year + '&month=' + month + '&format=csv';
    if (personnelId) {
        url += '&personnelId=' + personnelId;
    }
    
    // Create a temporary link to download the file
    var link = document.createElement('a');
    link.href = url;
    link.download = reportType + '-' + year + '-' + (month < 10 ? '0' : '') + month + '.csv';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    this.showSuccess('CSV download started');
};

ReportsManager.prototype.formatCurrency = function(amount) {
    return '$' + parseFloat(amount || 0).toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
};

ReportsManager.prototype.formatPercentage = function(value) {
    return (parseFloat(value || 0) * 100).toFixed(1) + '%';
};

ReportsManager.prototype.showLoading = function(show) {
    if (show) {
        $('#reportsLoadingIndicator').show();
    } else {
        $('#reportsLoadingIndicator').hide();
    }
};

ReportsManager.prototype.showError = function(message) {
    var alertHtml = '<div class="alert alert-danger alert-dismissible fade show">' +
        '<i class="bi bi-exclamation-triangle"></i> ' + message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
    '</div>';
    
    var $alert = $(alertHtml);
    $('.container-fluid').prepend($alert);
    
    setTimeout(function() {
        $alert.fadeOut(function() {
            $alert.remove();
        });
    }, 5000);
};

ReportsManager.prototype.showSuccess = function(message) {
    var alertHtml = '<div class="alert alert-success alert-dismissible fade show">' +
        '<i class="bi bi-check-circle"></i> ' + message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
    '</div>';
    
    var $alert = $(alertHtml);
    $('.container-fluid').prepend($alert);
    
    setTimeout(function() {
        $alert.fadeOut(function() {
            $alert.remove();
        });
    }, 3000);
};

// Initialize when DOM is loaded
$(document).ready(function() {
    window.reportsManager = new ReportsManager();
});
