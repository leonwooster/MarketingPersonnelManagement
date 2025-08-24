// API Client for Marketing Personnel Management
class ApiClient {
    constructor() {
        this.baseUrl = API_CONFIG.baseUrl;
    }

    async request(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const config = {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        };

        try {
            const response = await fetch(url, config);
            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || `HTTP error! status: ${response.status}`);
            }

            return data;
        } catch (error) {
            console.error('API request failed:', error);
            throw error;
        }
    }

    // Personnel API methods
    async getAllPersonnel() {
        return await this.request(API_CONFIG.endpoints.personnel);
    }

    async getPersonnelById(id) {
        return await this.request(`${API_CONFIG.endpoints.personnel}/${id}`);
    }

    async createPersonnel(personnelData) {
        return await this.request(API_CONFIG.endpoints.personnel, {
            method: 'POST',
            body: JSON.stringify(personnelData)
        });
    }

    async updatePersonnel(id, personnelData) {
        return await this.request(`${API_CONFIG.endpoints.personnel}/${id}`, {
            method: 'PUT',
            body: JSON.stringify(personnelData)
        });
    }

    async deletePersonnel(id, confirm = true) {
        return await this.request(`${API_CONFIG.endpoints.personnel}/${id}?confirm=${confirm}`, {
            method: 'DELETE'
        });
    }

    // Commission Profiles API methods
    async getAllCommissionProfiles() {
        return await this.request(API_CONFIG.endpoints.commissionProfiles);
    }
}

// Global API client instance
const apiClient = new ApiClient();