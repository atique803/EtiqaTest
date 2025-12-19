import React, { useState } from 'react';
import { employeeService } from '../services/api';
import { useNavigate } from 'react-router-dom';
import './SalaryCalculator.css';

const SalaryCalculator = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    employeeNumber: '',
    startDate: '',
    endDate: ''
  });
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    setError(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setResult(null);

    if (!formData.employeeNumber.trim()) {
      setError('Employee number is required');
      return;
    }
    if (!formData.startDate) {
      setError('Start date is required');
      return;
    }
    if (!formData.endDate) {
      setError('End date is required');
      return;
    }
    if (new Date(formData.startDate) > new Date(formData.endDate)) {
      setError('Start date must be before or equal to end date');
      return;
    }

    try {
      setLoading(true);
      const response = await employeeService.calculateSalary(formData);
      setResult(response.data);
    } catch (err) {
      setError('Calculation failed: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  const handleReset = () => {
    setFormData({
      employeeNumber: '',
      startDate: '',
      endDate: ''
    });
    setResult(null);
    setError(null);
  };

  return (
    <div className="salary-calculator-container">
      <div className="header">
        <h1>Salary Calculator</h1>
        <button className="btn btn-secondary" onClick={() => navigate('/')}>
          Back to List
        </button>
      </div>

      <div className="calculator-content">
        <div className="calculator-form-card">
          <h3>Enter Calculation Details</h3>
          
          {error && <div className="error-message">{error}</div>}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Employee Number *</label>
              <input
                type="text"
                name="employeeNumber"
                value={formData.employeeNumber}
                onChange={handleChange}
                placeholder="e.g., RAZ-12340-14MAY1994"
                required
              />
            </div>

            <div className="form-group">
              <label>Start Date *</label>
              <input
                type="date"
                name="startDate"
                value={formData.startDate}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>End Date *</label>
              <input
                type="date"
                name="endDate"
                value={formData.endDate}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-actions">
              <button type="submit" className="btn btn-primary" disabled={loading}>
                {loading ? 'Calculating...' : 'Calculate Salary'}
              </button>
              <button type="button" className="btn btn-secondary" onClick={handleReset}>
                Reset
              </button>
            </div>
          </form>
        </div>

        {result && (
          <div className="result-card">
            <h3>Salary Calculation Result</h3>
            
            <div className="result-section">
              <h4>Employee Information</h4>
              <div className="result-row">
                <span className="label">Employee Number:</span>
                <span className="value">{result.employeeNumber}</span>
              </div>
              <div className="result-row">
                <span className="label">Employee Name:</span>
                <span className="value">{result.employeeName}</span>
              </div>
              <div className="result-row">
                <span className="label">Daily Rate:</span>
                <span className="value">RM {result.dailyRate.toFixed(2)}</span>
              </div>
              <div className="result-row">
                <span className="label">Working Days:</span>
                <span className="value">{result.workingDaysList.join(', ')}</span>
              </div>
            </div>

            <div className="result-section">
              <h4>Calculation Period</h4>
              <div className="result-row">
                <span className="label">Start Date:</span>
                <span className="value">{new Date(result.startDate).toLocaleDateString()}</span>
              </div>
              <div className="result-row">
                <span className="label">End Date:</span>
                <span className="value">{new Date(result.endDate).toLocaleDateString()}</span>
              </div>
            </div>

            <div className="result-section">
              <h4>Calculation Breakdown</h4>
              <div className="result-row">
                <span className="label">Total Working Days:</span>
                <span className="value">{result.totalWorkingDays} days</span>
              </div>
              <div className="result-row">
                <span className="label">Working Days Pay:</span>
                <span className="value">
                  {result.totalWorkingDays} days × 2 × RM {result.dailyRate.toFixed(2)} = RM {(result.totalWorkingDays * 2 * result.dailyRate).toFixed(2)}
                </span>
              </div>
              {result.birthdayBonus > 0 && (
                <>
                  <div className="result-row highlight">
                    <span className="label">Birthday Bonus:</span>
                    <span className="value">
                      {result.birthdayBonus} day × 2 × RM {result.dailyRate.toFixed(2)} = RM {(result.birthdayBonus * 2 * result.dailyRate).toFixed(2)}
                    </span>
                  </div>
                  <div className="result-row">
                    <span className="label">Birthday Date:</span>
                    <span className="value">{result.birthdayDate}</span>
                  </div>
                </>
              )}
              <div className="result-row">
                <span className="label">Total Days Paid:</span>
                <span className="value">{result.totalDaysPaid} days</span>
              </div>
            </div>

            <div className="result-total">
              <span className="label">Take-Home Pay:</span>
              <span className="value">RM {result.takeHomePay.toFixed(2)}</span>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default SalaryCalculator;
