import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import EmployeeList from './components/EmployeeList';
import EmployeeForm from './components/EmployeeForm';
import EmployeeDetail from './components/EmployeeDetail';
import SalaryCalculator from './components/SalaryCalculator';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<EmployeeList />} />
          <Route path="/employees/new" element={<EmployeeForm />} />
          <Route path="/employees/edit/:id" element={<EmployeeForm />} />
          <Route path="/employees/:id" element={<EmployeeDetail />} />
          <Route path="/calculate-salary" element={<SalaryCalculator />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
