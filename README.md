# Payroll System - ETIQA Assessment

A full-stack employee payroll management system with salary calculation capabilities, built with .NET 8 and React.

## 📋 Table of Contents

- [Technologies](#technologies)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Database Setup](#database-setup)
- [Backend Setup](#backend-setup)
- [Frontend Setup](#frontend-setup)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Features](#features)
- [Business Rules](#business-rules)

---

## 🛠 Technologies

### Backend
- **Framework**: ASP.NET Core 8.0 (Web API)
- **ORM**: Dapper 2.1.66 (Micro ORM)
- **Database**: SQL Server 2022
- **Migrations**: DbUp 5.0.41
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)

### Frontend
- **Framework**: React 19.2.3
- **HTTP Client**: Axios 1.13.2
- **Routing**: React Router DOM 7.11.0
- **Styling**: CSS3

---

## 🏗 Architecture

```
backend/
├── src/
│   ├── PayrollSystem.Domain/          # Entities, Value Objects, Interfaces
│   ├── PayrollSystem.Application/      # DTOs, Services, Business Logic
│   ├── PayrollSystem.Infrastructure/   # Repositories, Data Access, Migrations
│   └── PayrollSystem.API/              # Controllers, Program.cs, Configuration

frontend/
├── src/
│   ├── components/                     # React Components
│   ├── services/                       # API Integration
│   └── App.js                          # Main App & Routing
└── public/
```

### Design Patterns
- **Repository Pattern**: Data access abstraction
- **Service Layer Pattern**: Business logic separation
- **DTO Pattern**: Data transfer objects
- **Clean Architecture**: Dependency inversion

---

## 📦 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- SQL Server 2022 (via Docker)

---

## 🗄 Database Setup

### 1. Start SQL Server Container

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrongPassword123!" \
  -p 1433:1433 --name sql1 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Verify Container

```bash
docker ps
docker logs sql1
```

### 3. Connection String

Update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=PayrollSystemDb;User ID=sa;Password=YourStrongPassword123!;Encrypt=False;Trust Server Certificate=True"
  }
}
```

### 4. Database Migrations

Migrations run automatically on application startup via DbUp. Scripts located in:
```
backend/src/PayrollSystem.Infrastructure/Migrations/Scripts/
```

---

## 🚀 Backend Setup

### 1. Navigate to Backend

```bash
cd backend/src/PayrollSystem.API
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build Project

```bash
dotnet build
```

### 4. Run API

```bash
dotnet run
```

API will be available at:
- **HTTP**: http://localhost:5211
- **Swagger**: http://localhost:5211/swagger

---

## 🎨 Frontend Setup

### 1. Navigate to Frontend

```bash
cd frontend
```

### 2. Install Dependencies

```bash
npm install
```

### 3. Start Development Server

```bash
npm start
```

Frontend will open at: **http://localhost:3000**

---

## 📡 API Documentation

### Base URL
```
http://localhost:5211/api
```

### Employees Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/employees` | Get all employees |
| GET | `/employees/{id}` | Get employee by ID |
| GET | `/employees/search?query={query}` | Search employees |
| POST | `/employees` | Create new employee |
| PUT | `/employees/{id}` | Update employee |
| DELETE | `/employees/{id}` | Delete employee |
| PUT | `/employees/{id}/archive` | Archive employee |
| PUT | `/employees/{id}/unarchive` | Unarchive employee |
| POST | `/employees/calculate-salary` | Calculate salary |

### Skillsets Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/skillsets` | Get all skillsets |
| GET | `/skillsets/{id}` | Get skillset by ID |
| POST | `/skillsets` | Create new skillset |
| PUT | `/skillsets/{id}` | Update skillset |
| DELETE | `/skillsets/{id}` | Delete skillset |

### Sample Requests

#### Create Employee

```json
POST /api/employees
{
  "employeeName": "Muhammad Atique",
  "nationalIdentificationNumber": "941210140123",
  "dateOfBirth": "1994-12-10",
  "contactNumber": "+60123456789",
  "residenceAddress": "123 Main St, Kuala Lumpur",
  "dailyRate": 150.00,
  "skillsetIds": [1, 2, 3],
  "workingDays": [1, 2, 3, 4, 5]
}
```

#### Calculate Salary

```json
POST /api/employees/calculate-salary
{
  "employeeNumber": "RAZ-12340-10DEC1994",
  "startDate": "2024-01-01",
  "endDate": "2024-01-31"
}
```

**Response:**
```json
{
  "employeeNumber": "RAZ-12340-10DEC1994",
  "employeeName": "Muhammad Atique",
  "startDate": "2024-01-01",
  "endDate": "2024-01-31",
  "dailyRate": 150.00,
  "workingDays": [1, 2, 3, 4, 5],
  "workingDaysList": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"],
  "totalWorkingDays": 23,
  "birthdayBonus": 1,
  "birthdayDate": "2024-01-10",
  "totalDaysPaid": 24,
  "takeHomePay": 7200.00
}
```

---

## 🗃 Database Schema

### Employees Table

```sql
CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeNumber VARCHAR(50) UNIQUE NOT NULL,
    EmployeeName NVARCHAR(200) NOT NULL,
    NationalIdentificationNumber VARCHAR(20) NOT NULL,
    DateOfBirth DATE NOT NULL,
    ContactNumber VARCHAR(20) NOT NULL,
    ResidenceAddress NVARCHAR(500) NOT NULL,
    DailyRate DECIMAL(18,2) NOT NULL,
    IsArchived BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
```

### Skillsets Table

```sql
CREATE TABLE Skillsets (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) UNIQUE NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
```

### EmployeeSkillsets Table (Many-to-Many)

```sql
CREATE TABLE EmployeeSkillsets (
    EmployeeId INT NOT NULL,
    SkillsetId INT NOT NULL,
    PRIMARY KEY (EmployeeId, SkillsetId),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    FOREIGN KEY (SkillsetId) REFERENCES Skillsets(Id) ON DELETE CASCADE
);
```

### EmployeeWorkingDays Table

```sql
CREATE TABLE EmployeeWorkingDays (
    EmployeeId INT NOT NULL,
    DayOfWeek INT NOT NULL,  -- 0=Sunday, 1=Monday, ..., 6=Saturday
    PRIMARY KEY (EmployeeId, DayOfWeek),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);
```

### Pre-seeded Skillsets

1. .NET Core
2. React
3. SQL Server
4. JavaScript
5. C#
6. Azure
7. Docker
8. Git

---

## ✨ Features

### Employee Management
- ✅ View all employees with status (Active/Archived)
- ✅ Search by employee number or name
- ✅ Create new employees with validation
- ✅ Update employee details
- ✅ Delete employees
- ✅ Archive/Unarchive employees
- ✅ View detailed employee information

### Skillset Management
- ✅ Multi-select skillsets for employees
- ✅ Pre-seeded with 8 common skillsets
- ✅ CRUD operations for skillsets

### Salary Calculation
- ✅ Calculate salary for date range
- ✅ Automatic working day detection
- ✅ Birthday bonus calculation
- ✅ Detailed breakdown display
- ✅ Real-time calculation

### UI/UX
- ✅ Responsive design
- ✅ Form validation
- ✅ Loading states
- ✅ Error handling
- ✅ Status badges
- ✅ Intuitive navigation

---

## 📐 Business Rules

### Employee Number Format
```
{Initials}-{5-digit-sequence}-{ddMMMyyyy}
Example: RAZ-12340-10DEC1994
```

- **Initials**: First 3 uppercase letters of employee name
- **Sequence**: Auto-incremented 5-digit number
- **Date**: Date of birth in ddMMMyyyy format

### Working Days
- 0 = Sunday
- 1 = Monday
- 2 = Tuesday
- 3 = Wednesday
- 4 = Thursday
- 5 = Friday
- 6 = Saturday

### Salary Calculation Formula

```
Total Pay = (Working Days × 2 × Daily Rate) + Birthday Bonus

Birthday Bonus = 1 day × 2 × Daily Rate (if birthday falls in period)
```

**Example:**
- Daily Rate: RM 150
- Working Days in Period: 23 days
- Birthday in Period: Yes
- **Calculation:**
  - Regular Pay: 23 × 2 × 150 = RM 6,900
  - Birthday Bonus: 1 × 2 × 150 = RM 300
  - **Total: RM 7,200**

---

## 🧪 Testing

Test the API using Swagger UI:

1. Navigate to http://localhost:5211/swagger
2. Use the interactive UI to test all endpoints
3. All DTOs and responses are fully documented

---

## 🔒 Security Notes

- **Development Only**: Current setup uses simple authentication
- **Connection String**: Stored in appsettings (use secrets in production)
- **CORS**: Configured for localhost:3000 (adjust for production)
- **SQL Injection**: Protected via Dapper parameterized queries

---

## 📝 Notes

- Database migrations run automatically on startup
- Employee numbers are generated server-side
- All dates are stored in UTC
- Soft delete implemented via `IsArchived` flag
- Cascade delete enabled for related records

---

## 👤 Author

**Muhammad Atique**  
ETIQA IT Assessment - December 2024

---

## 📄 License

This project is for assessment purposes only.
