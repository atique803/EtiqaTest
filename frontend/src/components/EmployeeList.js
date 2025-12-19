import React, { useState, useEffect } from 'react';
import { employeeService } from '../services/api';
import { useNavigate } from 'react-router-dom';
import './EmployeeList.css';

const EmployeeList = () => {
  const [employees, setEmployees] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [includeArchived, setIncludeArchived] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    loadEmployees();
  }, [includeArchived]);

  const loadEmployees = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await employeeService.getAll(includeArchived);
      setEmployees(response.data);
    } catch (err) {
      setError('Failed to load employees: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      loadEmployees();
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const response = await employeeService.search(searchTerm, includeArchived);
      setEmployees(response.data);
    } catch (err) {
      setError('Search failed: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleArchive = async (id, isArchived) => {
    try {
      if (isArchived) {
        await employeeService.unarchive(id);
      } else {
        await employeeService.archive(id);
      }
      loadEmployees();
    } catch (err) {
      setError(`Failed to ${isArchived ? 'unarchive' : 'archive'} employee: ` + err.message);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this employee?')) {
      try {
        await employeeService.delete(id);
        loadEmployees();
      } catch (err) {
        setError('Failed to delete employee: ' + err.message);
      }
    }
  };

  return (
    <div className="employee-list-container">
      <div className="header">
        <h1>Employee Management System</h1>
        <div style={{ display: 'flex', gap: '10px' }}>
          <button className="btn btn-success" onClick={() => navigate('/calculate-salary')}>
            💰 Calculate Salary
          </button>
          <button className="btn btn-primary" onClick={() => navigate('/employees/new')}>
            + Add New Employee
          </button>
        </div>
      </div>

      <div className="search-bar">
        <input
          type="text"
          placeholder="Search by employee number or name..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
        />
        <button className="btn btn-search" onClick={handleSearch}>Search</button>
        <button className="btn btn-secondary" onClick={() => { setSearchTerm(''); loadEmployees(); }}>
          Clear
        </button>
        <label className="checkbox-label">
          <input
            type="checkbox"
            checked={includeArchived}
            onChange={(e) => setIncludeArchived(e.target.checked)}
          />
          Include Archived
        </label>
      </div>

      {error && <div className="error-message">{error}</div>}

      {loading ? (
        <div className="loading">Loading...</div>
      ) : (
        <div className="table-container">
          <table className="employee-table">
            <thead>
              <tr>
                <th>Employee Number</th>
                <th>Name</th>
                <th>Date of Birth</th>
                <th>Daily Rate</th>
                <th>Skillsets</th>
                <th>Working Days</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {employees.length === 0 ? (
                <tr>
                  <td colSpan="8" className="no-data">No employees found</td>
                </tr>
              ) : (
                employees.map((employee) => (
                  <tr key={employee.id} className={employee.isArchived ? 'archived-row' : ''}>
                    <td>{employee.employeeNumber}</td>
                    <td>{employee.employeeName}</td>
                    <td>{new Date(employee.dateOfBirth).toLocaleDateString()}</td>
                    <td>RM {employee.dailyRate.toFixed(2)}</td>
                    <td>
                      {employee.skillsets.map(s => s.name).join(', ')}
                    </td>
                    <td>
                      {employee.workingDays.map(d => ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'][d]).join(', ')}
                    </td>
                    <td>
                      <span className={`status-badge ${employee.isArchived ? 'archived' : 'active'}`}>
                        {employee.isArchived ? 'Archived' : 'Active'}
                      </span>
                    </td>
                    <td className="actions">
                      <button 
                        className="btn btn-small btn-info"
                        onClick={() => navigate(`/employees/${employee.id}`)}
                      >
                        View
                      </button>
                      <button 
                        className="btn btn-small btn-edit"
                        onClick={() => navigate(`/employees/edit/${employee.id}`)}
                      >
                        Edit
                      </button>
                      <button 
                        className="btn btn-small btn-warning"
                        onClick={() => handleArchive(employee.id, employee.isArchived)}
                      >
                        {employee.isArchived ? 'Unarchive' : 'Archive'}
                      </button>
                      <button 
                        className="btn btn-small btn-danger"
                        onClick={() => handleDelete(employee.id)}
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      <div className="footer-actions">
        <button 
          className="btn btn-secondary" 
          onClick={() => navigate('/calculate-salary')}
        >
          Calculate Salary
        </button>
      </div>
    </div>
  );
};

export default EmployeeList;
