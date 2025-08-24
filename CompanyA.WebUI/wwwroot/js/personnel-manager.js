// Personnel Management SPA
class PersonnelManager {
    constructor() {
        this.personnel = [];
        this.commissionProfiles = [];
        this.currentPersonnel = null;
        this.isEditMode = false;
        this.init();
    }

    async init() {
        this.setupEventListeners();
        await this.loadCommissionProfiles();
        await this.loadPersonnel();
        this.renderPersonnelGrid();
    }

    setupEventListeners() {
        // Add Personnel button
        document.getElementById('addPersonnelBtn').addEventListener('click', () => {
            this.openPersonnelModal();
        });

        // Modal form submit
        document.getElementById('personnelForm').addEventListener('submit', (e) => {
            e.preventDefault();
            this.handleFormSubmit();
        });

        // Modal close buttons
        document.querySelectorAll('[data-bs-dismiss="modal"]').forEach(btn => {
            btn.addEventListener('click', () => {
                this.closePersonnelModal();
            });
        });

        // Search functionality
        document.getElementById('searchInput').addEventListener('input', (e) => {
            this.filterPersonnel(e.target.value);
        });
    }

    async loadPersonnel() {
        try {
            const response = await apiClient.getAllPersonnel();
            if (response.success) {
                this.personnel = response.data;
            } else {
                this.showError('Failed to load personnel: ' + response.message);
            }
        } catch (error) {
            this.showError('Error loading personnel: ' + error.message);
        }
    }

    async loadCommissionProfiles() {
        try {
            // For now, create mock data since CommissionProfile API isn't implemented yet
            this.commissionProfiles = [
                { id: 1, profileName: 1, commissionFixed: 500.00, commissionPercentage: 0.05 },
                { id: 2, profileName: 2, commissionFixed: 750.00, commissionPercentage: 0.03 },
                { id: 3, profileName: 3, commissionFixed: 300.00, commissionPercentage: 0.08 }
            ];
        } catch (error) {
            this.showError('Error loading commission profiles: ' + error.message);
        }
    }

    renderPersonnelGrid() {
        const tbody = document.getElementById('personnelTableBody');
        tbody.innerHTML = '';

        if (this.personnel.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center">No personnel found</td></tr>';
            return;
        }

        this.personnel.forEach(person => {
            const row = this.createPersonnelRow(person);
            tbody.appendChild(row);
        });
    }

    createPersonnelRow(person) {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${this.escapeHtml(person.name)}</td>
            <td>${person.age}</td>
            <td>${this.escapeHtml(person.phone)}</td>
            <td>${this.escapeHtml(person.bankName || '-')}</td>
            <td>${this.escapeHtml(person.bankAccountNo || '-')}</td>
            <td>Profile ${person.commissionProfileId}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary me-1" onclick="personnelManager.editPersonnel(${person.id})">
                    <i class="bi bi-pencil"></i> Edit
                </button>
                <button class="btn btn-sm btn-outline-danger" onclick="personnelManager.confirmDeletePersonnel(${person.id}, '${this.escapeHtml(person.name)}')">
                    <i class="bi bi-trash"></i> Delete
                </button>
            </td>
        `;
        return row;
    }

    openPersonnelModal(personnel = null) {
        this.currentPersonnel = personnel;
        this.isEditMode = personnel !== null;
        
        const modal = new bootstrap.Modal(document.getElementById('personnelModal'));
        const modalTitle = document.getElementById('personnelModalLabel');
        const form = document.getElementById('personnelForm');
        
        modalTitle.textContent = this.isEditMode ? 'Edit Personnel' : 'Add New Personnel';
        
        // Reset form
        form.reset();
        this.clearValidationErrors();
        
        // Populate commission profile dropdown
        this.populateCommissionProfiles();
        
        if (this.isEditMode && personnel) {
            this.populateForm(personnel);
        }
        
        modal.show();
    }

    closePersonnelModal() {
        this.currentPersonnel = null;
        this.isEditMode = false;
        this.clearValidationErrors();
    }

    populateCommissionProfiles() {
        const select = document.getElementById('commissionProfileId');
        select.innerHTML = '<option value="">Select Commission Profile</option>';
        
        this.commissionProfiles.forEach(profile => {
            const option = document.createElement('option');
            option.value = profile.id;
            option.textContent = `Profile ${profile.profileName} (Fixed: $${profile.commissionFixed}, Rate: ${(profile.commissionPercentage * 100).toFixed(1)}%)`;
            select.appendChild(option);
        });
    }

    populateForm(personnel) {
        document.getElementById('name').value = personnel.name;
        document.getElementById('age').value = personnel.age;
        document.getElementById('phone').value = personnel.phone;
        document.getElementById('bankName').value = personnel.bankName || '';
        document.getElementById('bankAccountNo').value = personnel.bankAccountNo || '';
        document.getElementById('commissionProfileId').value = personnel.commissionProfileId;
    }

    async handleFormSubmit() {
        const formData = this.getFormData();
        
        // Client-side validation
        if (!this.validateForm(formData)) {
            return;
        }

        try {
            let response;
            if (this.isEditMode) {
                response = await apiClient.updatePersonnel(this.currentPersonnel.id, formData);
            } else {
                response = await apiClient.createPersonnel(formData);
            }

            if (response.success) {
                this.showSuccess(response.message || (this.isEditMode ? 'Personnel updated successfully' : 'Personnel created successfully'));
                bootstrap.Modal.getInstance(document.getElementById('personnelModal')).hide();
                await this.loadPersonnel();
                this.renderPersonnelGrid();
            } else {
                this.showValidationErrors(response.errors || [response.message]);
            }
        } catch (error) {
            this.showError('Error saving personnel: ' + error.message);
        }
    }

    getFormData() {
        return {
            name: document.getElementById('name').value.trim(),
            age: parseInt(document.getElementById('age').value),
            phone: document.getElementById('phone').value.trim(),
            bankName: document.getElementById('bankName').value.trim() || null,
            bankAccountNo: document.getElementById('bankAccountNo').value.trim() || null,
            commissionProfileId: parseInt(document.getElementById('commissionProfileId').value)
        };
    }

    validateForm(data) {
        this.clearValidationErrors();
        const errors = [];

        if (!data.name) {
            errors.push({ field: 'name', message: 'Name is required' });
        } else if (data.name.length > UI_CONFIG.validation.maxNameLength) {
            errors.push({ field: 'name', message: `Name cannot exceed ${UI_CONFIG.validation.maxNameLength} characters` });
        }

        if (!data.age || data.age < UI_CONFIG.validation.minAge) {
            errors.push({ field: 'age', message: `Age must be ${UI_CONFIG.validation.minAge} or older` });
        }

        if (!data.phone) {
            errors.push({ field: 'phone', message: 'Phone is required' });
        } else if (data.phone.length > UI_CONFIG.validation.maxPhoneLength) {
            errors.push({ field: 'phone', message: `Phone cannot exceed ${UI_CONFIG.validation.maxPhoneLength} characters` });
        }

        if (!data.commissionProfileId) {
            errors.push({ field: 'commissionProfileId', message: 'Commission profile is required' });
        }

        if (data.bankName && data.bankName.length > UI_CONFIG.validation.maxBankNameLength) {
            errors.push({ field: 'bankName', message: `Bank name cannot exceed ${UI_CONFIG.validation.maxBankNameLength} characters` });
        }

        if (data.bankAccountNo && data.bankAccountNo.length > UI_CONFIG.validation.maxBankAccountLength) {
            errors.push({ field: 'bankAccountNo', message: `Bank account number cannot exceed ${UI_CONFIG.validation.maxBankAccountLength} characters` });
        }

        if (errors.length > 0) {
            this.showValidationErrors(errors);
            return false;
        }

        return true;
    }

    showValidationErrors(errors) {
        errors.forEach(error => {
            if (typeof error === 'string') {
                this.showError(error);
            } else if (error.field) {
                const field = document.getElementById(error.field);
                if (field) {
                    field.classList.add('is-invalid');
                    const feedback = field.parentNode.querySelector('.invalid-feedback');
                    if (feedback) {
                        feedback.textContent = error.message;
                    }
                }
            }
        });
    }

    clearValidationErrors() {
        document.querySelectorAll('.is-invalid').forEach(field => {
            field.classList.remove('is-invalid');
        });
        document.querySelectorAll('.invalid-feedback').forEach(feedback => {
            feedback.textContent = '';
        });
    }

    async editPersonnel(id) {
        const personnel = this.personnel.find(p => p.id === id);
        if (personnel) {
            this.openPersonnelModal(personnel);
        }
    }

    confirmDeletePersonnel(id, name) {
        if (confirm(`Are you sure you want to delete ${name}?\n\nThis will also delete all associated sales records. This action cannot be undone.`)) {
            this.deletePersonnel(id);
        }
    }

    async deletePersonnel(id) {
        try {
            const response = await apiClient.deletePersonnel(id, true);
            if (response.success) {
                this.showSuccess(response.message || 'Personnel deleted successfully');
                await this.loadPersonnel();
                this.renderPersonnelGrid();
            } else {
                this.showError('Failed to delete personnel: ' + response.message);
            }
        } catch (error) {
            this.showError('Error deleting personnel: ' + error.message);
        }
    }

    filterPersonnel(searchTerm) {
        const filteredPersonnel = this.personnel.filter(person =>
            person.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            person.phone.includes(searchTerm) ||
            (person.bankName && person.bankName.toLowerCase().includes(searchTerm.toLowerCase()))
        );
        
        this.renderFilteredPersonnel(filteredPersonnel);
    }

    renderFilteredPersonnel(filteredPersonnel) {
        const tbody = document.getElementById('personnelTableBody');
        tbody.innerHTML = '';

        if (filteredPersonnel.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center">No personnel found matching your search</td></tr>';
            return;
        }

        filteredPersonnel.forEach(person => {
            const row = this.createPersonnelRow(person);
            tbody.appendChild(row);
        });
    }

    showSuccess(message) {
        this.showAlert(message, 'success');
    }

    showError(message) {
        this.showAlert(message, 'danger');
    }

    showAlert(message, type) {
        const alertContainer = document.getElementById('alertContainer');
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show`;
        alert.innerHTML = `
            ${this.escapeHtml(message)}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        alertContainer.appendChild(alert);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (alert.parentNode) {
                alert.remove();
            }
        }, 5000);
    }

    escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.personnelManager = new PersonnelManager();
});