-- Create Skillsets table
CREATE TABLE Skillsets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Create index for Skillsets
CREATE INDEX IX_Skillsets_Name ON Skillsets(Name);
