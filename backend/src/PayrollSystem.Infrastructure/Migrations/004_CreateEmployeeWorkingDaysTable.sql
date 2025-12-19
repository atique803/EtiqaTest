-- Create EmployeeWorkingDays table
CREATE TABLE EmployeeWorkingDays (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    DayOfWeek INT NOT NULL, -- 0 = Sunday, 1 = Monday, etc.
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_EmployeeWorkingDays_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_EmployeeWorkingDays UNIQUE (EmployeeId, DayOfWeek),
    CONSTRAINT CK_DayOfWeek CHECK (DayOfWeek BETWEEN 0 AND 6)
);

-- Create index for EmployeeWorkingDays
CREATE INDEX IX_EmployeeWorkingDays_EmployeeId ON EmployeeWorkingDays(EmployeeId);
