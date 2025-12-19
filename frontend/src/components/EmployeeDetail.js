import React, { useState, useEffect } from 'react';
import { employeeService } from '../services/api';
import { useNavigate, useParams } from 'react-router-dom';
import './EmployeeDetail.css';

const EmployeeDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [employee, setEmployee] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadEmployee();
  }, [id]);

  const loadEmployee = async () => {
    try {
      setLoading(true);
      const response = await employeeService.getById(id);
      setEmployee(response.data);
    } catch (err) {
      setError('Failed to load employee: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading employee details...</div>;
  }

  if (error) {
    return (
      <div className="employee-detail-container">
        <div className="error-message">{error}</div>
        <button className="btn btn-secondary" onClick={() => navigate('/')}>
          Back to List
        </button>
      </div>
    );
  }

  if (!employee) {
    return null;
  }

  const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  return (
    <div className="employee-detail-container">
      <div className="header">
        <h1>Employee Details</h1>
        <div className="header-actions">
          <button className="btn btn-primary" onClick={() => navigate(`/employees/edit/${id}`)}>
            Edit
          </button>
          <button className="btn btn-secondary" onClick={() => navigate('/')}>
            Back to List
          </button>
        </div>
      </div>

      <div className="detail-card">
        <div className="detail-header">
          <div>
            <h2>{employee.employeeName}</h2>
            <p className="employee-number">{employee.employeeNumber}</p>
          </div>
          <span className={`status-badge ${employee.isArchived ? 'archived' : 'active'}`}>
            {employee.isArchived ? 'Archived' : 'Active'}
          </span>
        </div>

        <div className="detail-sections">
          <div className="detail-section">
            <h3>Personal Information</h3>
            <div className="detail-row">
              <span className="label">National ID:</span>
              <span className="value">{employee.nationalIdentificationNumber}</span>
            </div>
            <div className="detail-row">
              <span className="label">Date of Birth:</span>
              <span className="value">{new Date(employee.dateOfBirth).toLocaleDateString()}</span>
            </div>
          </div>

          <div className="detail-section">
            <h3>Contact Information</h3>
            <div className="detail-row">
              <span className="label">Phone:</span>
              <span className="value">{employee.contactNumber}</span>
            </div>
            <div className="detail-row">
              <span className="label">Address:</span>
              <span className="value">{employee.residenceAddress}</span>
            </div>
          </div>

          <div className="detail-section">
            <h3>Employment Details</h3>
            <div className="detail-row">
              <span className="label">Daily Rate:</span>
              <span className="value">RM {employee.dailyRate.toFixed(2)}</span>
            </div>
            <div className="detail-row">
              <span className="label">Working Days:</span>
              <span className="value">
                {employee.workingDays.map(d => dayNames[d]).join(', ')}
              </span>
            </div>
          </div>

          <div className="detail-section">
            <h3>Skillsets</h3>
            <div className="skillsets-list">
              {employee.skillsets.length > 0 ? (
                employee.skillsets.map(skillset => (
                  <span key={skillset.id} className="skillset-badge">
                    {skillset.name}
                  </span>
                ))
              ) : (
                <span className="no-data">No skillsets assigned</span>
              )}
            </div>
          </div>

          <div className="detail-section">
            <h3>Record Information</h3>
            <div className="detail-row">
              <span className="label">Created:</span>
              <span className="value">{new Date(employee.createdAt).toLocaleString()}</span>
            </div>
            <div className="detail-row">
              <span className="label">Last Updated:</span>
              <span className="value">{new Date(employee.updatedAt).toLocaleString()}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EmployeeDetail;
