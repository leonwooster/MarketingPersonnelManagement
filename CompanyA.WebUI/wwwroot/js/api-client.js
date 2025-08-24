// API Client for Marketing Personnel Management
function ApiClient() {
    this.baseUrl = API_CONFIG.baseUrl;
}

ApiClient.prototype.request = function(endpoint, options, callback) {
    var url = this.baseUrl + endpoint;
    var config = {
        url: url,
        method: options.method || 'GET',
        contentType: 'application/json',
        success: function(data) {
            callback(null, data);
        },
        error: function(xhr, status, error) {
            var errorMessage = 'HTTP error! status: ' + xhr.status;
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            console.error('API request failed:', errorMessage);
            callback(new Error(errorMessage));
        }
    };

    if (options.data) {
        config.data = JSON.stringify(options.data);
    }

    $.ajax(config);
};

// Personnel API methods
ApiClient.prototype.getAllPersonnel = function(callback) {
    this.request(API_CONFIG.endpoints.personnel, {}, callback);
};

ApiClient.prototype.getPersonnelById = function(id, callback) {
    this.request(API_CONFIG.endpoints.personnel + '/' + id, {}, callback);
};

ApiClient.prototype.createPersonnel = function(personnelData, callback) {
    this.request(API_CONFIG.endpoints.personnel, {
        method: 'POST',
        data: personnelData
    }, callback);
};

ApiClient.prototype.updatePersonnel = function(id, personnelData, callback) {
    this.request(API_CONFIG.endpoints.personnel + '/' + id, {
        method: 'PUT',
        data: personnelData
    }, callback);
};

ApiClient.prototype.deletePersonnel = function(id, confirm, callback) {
    if (typeof confirm === 'function') {
        callback = confirm;
        confirm = true;
    }
    this.request(API_CONFIG.endpoints.personnel + '/' + id + '?confirm=' + confirm, {
        method: 'DELETE'
    }, callback);
};

// Commission Profiles API methods
ApiClient.prototype.getAllCommissionProfiles = function(callback) {
    this.request(API_CONFIG.endpoints.commissionProfiles, {}, callback);
};

// Global API client instance
var apiClient = new ApiClient();