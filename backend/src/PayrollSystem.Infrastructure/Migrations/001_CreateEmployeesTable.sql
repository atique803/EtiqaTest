-- Create Employees table
CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeNumber NVARCHAR(50) NOT NULL UNIQUE,
    EmployeeName NVARCHAR(200) NOT NULL,
    NationalIdentificationNumber NVARCHAR(50) NOT NULL,
    ContactNumber NVARCHAR(20) NOT NULL,
    ResidenceAddress NVARCHAR(500) NOT NULL,
    DateOfBirth DATE NOT NULL,
    DailyRate DECIMAL(18, 2) NOT NULL,
    IsArchived BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Create indexes for Employees
CREATE INDEX IX_Employees_EmployeeNumber ON Employees(EmployeeNumber);
CREATE INDEX IX_Employees_EmployeeName ON Employees(EmployeeName);
CREATE INDEX IX_Employees_IsArchived ON Employees(IsArchived);
