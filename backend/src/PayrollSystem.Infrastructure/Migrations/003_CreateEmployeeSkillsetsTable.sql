-- Create EmployeeSkillsets junction table
CREATE TABLE EmployeeSkillsets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    SkillsetId INT NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_EmployeeSkillsets_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE,
    CONSTRAINT FK_EmployeeSkillsets_Skillset FOREIGN KEY (SkillsetId) REFERENCES Skillsets(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_EmployeeSkillsets UNIQUE (EmployeeId, SkillsetId)
);

-- Create indexes for EmployeeSkillsets
CREATE INDEX IX_EmployeeSkillsets_EmployeeId ON EmployeeSkillsets(EmployeeId);
CREATE INDEX IX_EmployeeSkillsets_SkillsetId ON EmployeeSkillsets(SkillsetId);
