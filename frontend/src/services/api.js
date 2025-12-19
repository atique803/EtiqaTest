import axios from 'axios';

const API_BASE_URL = 'http://localhost:5211/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Employee APIs
export const employeeService = {
  getAll: (includeArchived = false) => 
    api.get(`/employees?includeArchived=${includeArchived}`),
  
  getById: (id) => 
    api.get(`/employees/${id}`),
  
  getByNumber: (employeeNumber) => 
    api.get(`/employees/by-number/${employeeNumber}`),
  
  search: (searchTerm, includeArchived = false) => 
    api.get(`/employees/search?searchTerm=${searchTerm}&includeArchived=${includeArchived}`),
  
  create: (employee) => 
    api.post('/employees', employee),
  
  update: (id, employee) => 
    api.put(`/employees/${id}`, employee),
  
  delete: (id) => 
    api.delete(`/employees/${id}`),
  
  archive: (id) => 
    api.post(`/employees/${id}/archive`),
  
  unarchive: (id) => 
    api.post(`/employees/${id}/unarchive`),
  
  calculateSalary: (data) => 
    api.post('/employees/calculate-salary', data),
};

// Skillsets APIs
export const skillsetService = {
  getAll: () => 
    api.get('/skillsets'),
  
  getById: (id) => 
    api.get(`/skillsets/${id}`),
};

export default api;
