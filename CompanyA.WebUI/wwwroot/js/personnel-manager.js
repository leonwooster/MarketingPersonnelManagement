// Personnel Management SPA
function PersonnelManager() {
    this.personnel = [];
    this.commissionProfiles = [];
    this.currentPersonnel = null;
    this.isEditMode = false;
    this.init();
}

PersonnelManager.prototype.init = function() {
    var self = this;
    this.setupEventListeners();
    this.loadCommissionProfiles(function() {
        self.loadPersonnel(function() {
            self.renderPersonnelGrid();
        });
    });
};

PersonnelManager.prototype.setupEventListeners = function() {
    var self = this;
    
    // Add Personnel button
    $('#addPersonnelBtn').on('click', function() {
        self.openPersonnelModal();
    });

    // Modal form submit
    $('#personnelForm').on('submit', function(e) {
        e.preventDefault();
        self.handleFormSubmit();
    });

    // Modal close buttons
    $('[data-bs-dismiss="modal"]').on('click', function() {
        self.closePersonnelModal();
    });

    // Search functionality
    $('#searchInput').on('input', function() {
        self.filterPersonnel($(this).val());
    });
};

PersonnelManager.prototype.loadPersonnel = function(callback) {
    var self = this;
    apiClient.getAllPersonnel(function(error, response) {
        if (error) {
            self.showError('Error loading personnel: ' + error.message);
        } else if (response.success) {
            self.personnel = response.data;
        } else {
            self.showError('Failed to load personnel: ' + response.message);
        }
        if (callback) callback();
    });
};

PersonnelManager.prototype.loadCommissionProfiles = function(callback) {
    // For now, create mock data since CommissionProfile API isn't implemented yet
    this.commissionProfiles = [
        { id: 1, profileName: 1, commissionFixed: 500.00, commissionPercentage: 0.05 },
        { id: 2, profileName: 2, commissionFixed: 750.00, commissionPercentage: 0.03 },
        { id: 3, profileName: 3, commissionFixed: 300.00, commissionPercentage: 0.08 }
    ];
    if (callback) callback();
};

PersonnelManager.prototype.renderPersonnelGrid = function() {
    var $tbody = $('#personnelTableBody');
    $tbody.empty();

    if (this.personnel.length === 0) {
        $tbody.html('<tr><td colspan="7" class="text-center">No personnel found</td></tr>');
        return;
    }

    var self = this;
    $.each(this.personnel, function(index, person) {
        var row = self.createPersonnelRow(person);
        $tbody.append(row);
    });
};

PersonnelManager.prototype.createPersonnelRow = function(person) {
    var html = '<tr>' +
        '<td>' + this.escapeHtml(person.name) + '</td>' +
        '<td>' + person.age + '</td>' +
        '<td>' + this.escapeHtml(person.phone) + '</td>' +
        '<td>' + this.escapeHtml(person.bankName || '-') + '</td>' +
        '<td>' + this.escapeHtml(person.bankAccountNo || '-') + '</td>' +
        '<td>Profile ' + person.commissionProfileId + '</td>' +
        '<td>' +
            '<button class="btn btn-sm btn-outline-primary me-1" onclick="personnelManager.editPersonnel(' + person.id + ')">' +
                '<i class="bi bi-pencil"></i> Edit' +
            '</button>' +
            '<button class="btn btn-sm btn-outline-danger" onclick="personnelManager.confirmDeletePersonnel(' + person.id + ', \'' + this.escapeHtml(person.name) + '\')">' +
                '<i class="bi bi-trash"></i> Delete' +
            '</button>' +
        '</td>' +
    '</tr>';
    return $(html);
};

PersonnelManager.prototype.openPersonnelModal = function(personnel) {
    this.currentPersonnel = personnel || null;
    this.isEditMode = personnel !== null && personnel !== undefined;
    
    var modal = new bootstrap.Modal(document.getElementById('personnelModal'));
    var modalTitle = $('#personnelModalLabel');
    var form = $('#personnelForm');
    
    modalTitle.text(this.isEditMode ? 'Edit Personnel' : 'Add New Personnel');
    
    // Reset form
    form[0].reset();
    this.clearValidationErrors();
    
    // Populate commission profile dropdown
    this.populateCommissionProfiles();
    
    if (this.isEditMode && personnel) {
        this.populateForm(personnel);
    }
    
    modal.show();
};

PersonnelManager.prototype.closePersonnelModal = function() {
    this.currentPersonnel = null;
    this.isEditMode = false;
    this.clearValidationErrors();
};

PersonnelManager.prototype.populateCommissionProfiles = function() {
    var $select = $('#commissionProfileId');
    $select.html('<option value="">Select Commission Profile</option>');
    
    var self = this;
    $.each(this.commissionProfiles, function(index, profile) {
        var optionText = 'Profile ' + profile.profileName + ' (Fixed: $' + profile.commissionFixed + ', Rate: ' + (profile.commissionPercentage * 100).toFixed(1) + '%)';
        $select.append($('<option></option>').val(profile.id).text(optionText));
    });
};

PersonnelManager.prototype.populateForm = function(personnel) {
    $('#name').val(personnel.name);
    $('#age').val(personnel.age);
    $('#phone').val(personnel.phone);
    $('#bankName').val(personnel.bankName || '');
    $('#bankAccountNo').val(personnel.bankAccountNo || '');
    $('#commissionProfileId').val(personnel.commissionProfileId);
};

PersonnelManager.prototype.handleFormSubmit = function() {
    var formData = this.getFormData();
    
    // Client-side validation
    if (!this.validateForm(formData)) {
        return;
    }

    var self = this;
    if (this.isEditMode) {
        apiClient.updatePersonnel(this.currentPersonnel.id, formData, function(error, response) {
            self.handleFormResponse(error, response, 'Personnel updated successfully');
        });
    } else {
        apiClient.createPersonnel(formData, function(error, response) {
            self.handleFormResponse(error, response, 'Personnel created successfully');
        });
    }
};

PersonnelManager.prototype.handleFormResponse = function(error, response, successMessage) {
    if (error) {
        this.showError('Error saving personnel: ' + error.message);
    } else if (response.success) {
        this.showSuccess(response.message || successMessage);
        bootstrap.Modal.getInstance(document.getElementById('personnelModal')).hide();
        var self = this;
        this.loadPersonnel(function() {
            self.renderPersonnelGrid();
        });
    } else {
        this.showValidationErrors(response.errors || [response.message]);
    }
};

PersonnelManager.prototype.getFormData = function() {
    return {
        name: $('#name').val().trim(),
        age: parseInt($('#age').val()),
        phone: $('#phone').val().trim(),
        bankName: $('#bankName').val().trim() || null,
        bankAccountNo: $('#bankAccountNo').val().trim() || null,
        commissionProfileId: parseInt($('#commissionProfileId').val())
    };
};

PersonnelManager.prototype.validateForm = function(data) {
    this.clearValidationErrors();
    var errors = [];

    if (!data.name) {
        errors.push({ field: 'name', message: 'Name is required' });
    } else if (data.name.length > UI_CONFIG.validation.maxNameLength) {
        errors.push({ field: 'name', message: 'Name cannot exceed ' + UI_CONFIG.validation.maxNameLength + ' characters' });
    }

    if (!data.age || data.age < UI_CONFIG.validation.minAge) {
        errors.push({ field: 'age', message: 'Age must be ' + UI_CONFIG.validation.minAge + ' or older' });
    }

    if (!data.phone) {
        errors.push({ field: 'phone', message: 'Phone is required' });
    } else if (data.phone.length > UI_CONFIG.validation.maxPhoneLength) {
        errors.push({ field: 'phone', message: 'Phone cannot exceed ' + UI_CONFIG.validation.maxPhoneLength + ' characters' });
    }

    if (!data.commissionProfileId) {
        errors.push({ field: 'commissionProfileId', message: 'Commission profile is required' });
    }

    if (data.bankName && data.bankName.length > UI_CONFIG.validation.maxBankNameLength) {
        errors.push({ field: 'bankName', message: 'Bank name cannot exceed ' + UI_CONFIG.validation.maxBankNameLength + ' characters' });
    }

    if (data.bankAccountNo && data.bankAccountNo.length > UI_CONFIG.validation.maxBankAccountLength) {
        errors.push({ field: 'bankAccountNo', message: 'Bank account number cannot exceed ' + UI_CONFIG.validation.maxBankAccountLength + ' characters' });
    }

    if (errors.length > 0) {
        this.showValidationErrors(errors);
        return false;
    }

    return true;
};

PersonnelManager.prototype.showValidationErrors = function(errors) {
    var self = this;
    $.each(errors, function(index, error) {
        if (typeof error === 'string') {
            self.showError(error);
        } else if (error.field) {
            var $field = $('#' + error.field);
            if ($field.length > 0) {
                $field.addClass('is-invalid');
                var $feedback = $field.parent().find('.invalid-feedback');
                if ($feedback.length > 0) {
                    $feedback.text(error.message);
                }
            }
        }
    });
};

PersonnelManager.prototype.clearValidationErrors = function() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').text('');
};

PersonnelManager.prototype.editPersonnel = function(id) {
    var personnel = null;
    $.each(this.personnel, function(index, p) {
        if (p.id === id) {
            personnel = p;
            return false; // break
        }
    });
    if (personnel) {
        this.openPersonnelModal(personnel);
    }
};

PersonnelManager.prototype.confirmDeletePersonnel = function(id, name) {
    if (confirm('Are you sure you want to delete ' + name + '?\n\nThis will also delete all associated sales records. This action cannot be undone.')) {
        this.deletePersonnel(id);
    }
};

PersonnelManager.prototype.deletePersonnel = function(id) {
    var self = this;
    apiClient.deletePersonnel(id, true, function(error, response) {
        if (error) {
            self.showError('Error deleting personnel: ' + error.message);
        } else if (response.success) {
            self.showSuccess(response.message || 'Personnel deleted successfully');
            self.loadPersonnel(function() {
                self.renderPersonnelGrid();
            });
        } else {
            self.showError('Failed to delete personnel: ' + response.message);
        }
    });
};

PersonnelManager.prototype.filterPersonnel = function(searchTerm) {
    var filteredPersonnel = [];
    var searchLower = searchTerm.toLowerCase();
    
    $.each(this.personnel, function(index, person) {
        if (person.name.toLowerCase().indexOf(searchLower) !== -1 ||
            person.phone.indexOf(searchTerm) !== -1 ||
            (person.bankName && person.bankName.toLowerCase().indexOf(searchLower) !== -1)) {
            filteredPersonnel.push(person);
        }
    });
    
    this.renderFilteredPersonnel(filteredPersonnel);
};

PersonnelManager.prototype.renderFilteredPersonnel = function(filteredPersonnel) {
    var $tbody = $('#personnelTableBody');
    $tbody.empty();

    if (filteredPersonnel.length === 0) {
        $tbody.html('<tr><td colspan="7" class="text-center">No personnel found matching your search</td></tr>');
        return;
    }

    var self = this;
    $.each(filteredPersonnel, function(index, person) {
        var row = self.createPersonnelRow(person);
        $tbody.append(row);
    });
};

PersonnelManager.prototype.showSuccess = function(message) {
    this.showAlert(message, 'success');
};

PersonnelManager.prototype.showError = function(message) {
    this.showAlert(message, 'danger');
};

PersonnelManager.prototype.showAlert = function(message, type) {
    var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show">' +
        this.escapeHtml(message) +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
    '</div>';
    
    var $alert = $(alertHtml);
    $('#alertContainer').append($alert);

    // Auto-remove after 5 seconds
    setTimeout(function() {
        $alert.fadeOut(function() {
            $alert.remove();
        });
    }, 5000);
};

PersonnelManager.prototype.escapeHtml = function(text) {
    if (!text) return '';
    return $('<div>').text(text).html();
};

// Initialize when DOM is loaded
$(document).ready(function() {
    window.personnelManager = new PersonnelManager();
});