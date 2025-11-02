IF COL_LENGTH('dbo.Patients','Email') IS NULL
    ALTER TABLE dbo.Patients ADD Email NVARCHAR(256) NULL;
IF COL_LENGTH('dbo.Patients','Phone') IS NULL
    ALTER TABLE dbo.Patients ADD Phone NVARCHAR(64) NULL;
IF COL_LENGTH('dbo.Patients','ReferringDoctor') IS NULL
    ALTER TABLE dbo.Patients ADD ReferringDoctor NVARCHAR(256) NULL;
