import React, { useState, useEffect } from 'react';
import { employeeService, skillsetService } from '../services/api';
import { useNavigate, useParams } from 'react-router-dom';
import './EmployeeForm.css';

const EmployeeForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = !!id;

  const [formData, setFormData] = useState({
    employeeName: '',
    nationalIdentificationNumber: '',
    contactNumber: '',
    residenceAddress: '',
    dateOfBirth: '',
    dailyRate: '',
    skillsetIds: [],
    workingDays: []
  });

  const [availableSkillsets, setAvailableSkillsets] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const daysOfWeek = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' }
  ];

  useEffect(() => {
    loadSkillsets();
    if (isEditMode) {
      loadEmployee();
    }
  }, [id]);

  const loadSkillsets = async () => {
    try {
      const response = await skillsetService.getAll();
      setAvailableSkillsets(response.data);
    } catch (err) {
      setError('Failed to load skillsets: ' + err.message);
    }
  };

  const loadEmployee = async () => {
    try {
      setLoading(true);
      const response = await employeeService.getById(id);
      const employee = response.data;
      
      setFormData({
        employeeName: employee.employeeName,
        nationalIdentificationNumber: employee.nationalIdentificationNumber,
        contactNumber: employee.contactNumber,
        residenceAddress: employee.residenceAddress,
        dateOfBirth: employee.dateOfBirth.split('T')[0],
        dailyRate: employee.dailyRate,
        skillsetIds: employee.skillsets.map(s => s.id),
        workingDays: employee.workingDays
      });
    } catch (err) {
      setError('Failed to load employee: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSkillsetChange = (skillsetId) => {
    setFormData(prev => {
      const skillsetIds = prev.skillsetIds.includes(skillsetId)
        ? prev.skillsetIds.filter(id => id !== skillsetId)
        : [...prev.skillsetIds, skillsetId];
      return { ...prev, skillsetIds };
    });
  };

  const handleWorkingDayChange = (day) => {
    setFormData(prev => {
      const workingDays = prev.workingDays.includes(day)
        ? prev.workingDays.filter(d => d !== day)
        : [...prev.workingDays, day];
      return { ...prev, workingDays: workingDays.sort((a, b) => a - b) };
    });
  };

  const validateForm = () => {
    if (!formData.employeeName.trim()) {
      setError('Employee name is required');
      return false;
    }
    if (!formData.nationalIdentificationNumber.trim()) {
      setError('National ID is required');
      return false;
    }
    if (!formData.contactNumber.trim()) {
      setError('Contact number is required');
      return false;
    }
    if (!formData.residenceAddress.trim()) {
      setError('Address is required');
      return false;
    }
    if (!formData.dateOfBirth) {
      setError('Date of birth is required');
      return false;
    }
    if (!formData.dailyRate || formData.dailyRate <= 0) {
      setError('Daily rate must be greater than 0');
      return false;
    }
    if (formData.workingDays.length === 0) {
      setError('Please select at least one working day');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      
      const submitData = {
        ...formData,
        dailyRate: parseFloat(formData.dailyRate)
      };

      if (isEditMode) {
        submitData.id = parseInt(id);
        await employeeService.update(id, submitData);
        setSuccess('Employee updated successfully!');
      } else {
        await employeeService.create(submitData);
        setSuccess('Employee created successfully!');
      }

      setTimeout(() => {
        navigate('/');
      }, 1500);
    } catch (err) {
      setError('Failed to save employee: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  if (loading && isEditMode) {
    return <div className="loading">Loading employee data...</div>;
  }

  return (
    <div className="employee-form-container">
      <div className="form-header">
        <h1>{isEditMode ? 'Edit Employee' : 'Add New Employee'}</h1>
        <button className="btn btn-secondary" onClick={() => navigate('/')}>
          Back to List
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}
      {success && <div className="success-message">{success}</div>}

      <form onSubmit={handleSubmit} className="employee-form">
        <div className="form-section">
          <h3>Personal Information</h3>
          
          <div className="form-group">
            <label>Employee Name *</label>
            <input
              type="text"
              name="employeeName"
              value={formData.employeeName}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>National Identification Number *</label>
            <input
              type="text"
              name="nationalIdentificationNumber"
              value={formData.nationalIdentificationNumber}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Date of Birth *</label>
            <input
              type="date"
              name="dateOfBirth"
              value={formData.dateOfBirth}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-section">
          <h3>Contact Information</h3>
          
          <div className="form-group">
            <label>Contact Number *</label>
            <input
              type="tel"
              name="contactNumber"
              value={formData.contactNumber}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Residence Address *</label>
            <textarea
              name="residenceAddress"
              value={formData.residenceAddress}
              onChange={handleChange}
              rows="3"
              required
            />
          </div>
        </div>

        <div className="form-section">
          <h3>Employment Details</h3>
          
          <div className="form-group">
            <label>Daily Rate (RM) *</label>
            <input
              type="number"
              name="dailyRate"
              value={formData.dailyRate}
              onChange={handleChange}
              step="0.01"
              min="0"
              required
            />
          </div>

          <div className="form-group">
            <label>Skillsets</label>
            <div className="checkbox-grid">
              {availableSkillsets.map(skillset => (
                <label key={skillset.id} className="checkbox-item">
                  <input
                    type="checkbox"
                    checked={formData.skillsetIds.includes(skillset.id)}
                    onChange={() => handleSkillsetChange(skillset.id)}
                  />
                  {skillset.name}
                </label>
              ))}
            </div>
          </div>

          <div className="form-group">
            <label>Working Days *</label>
            <div className="checkbox-grid">
              {daysOfWeek.map(day => (
                <label key={day.value} className="checkbox-item">
                  <input
                    type="checkbox"
                    checked={formData.workingDays.includes(day.value)}
                    onChange={() => handleWorkingDayChange(day.value)}
                  />
                  {day.label}
                </label>
              ))}
            </div>
          </div>
        </div>

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : (isEditMode ? 'Update Employee' : 'Create Employee')}
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/')}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default EmployeeForm;
