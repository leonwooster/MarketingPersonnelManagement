// API Configuration
const API_CONFIG = {
    baseUrl: 'http://localhost:5041/api',
    endpoints: {
        personnel: '/personnel',
        commissionProfiles: '/commissionprofiles',
        sales: '/sales',
        reports: '/reports'
    }
};

// UI Configuration
const UI_CONFIG = {
    pagination: {
        itemsPerPage: 10
    },
    validation: {
        minAge: 19,
        maxNameLength: 50,
        maxPhoneLength: 20,
        maxBankNameLength: 20,
        maxBankAccountLength: 20
    }
};