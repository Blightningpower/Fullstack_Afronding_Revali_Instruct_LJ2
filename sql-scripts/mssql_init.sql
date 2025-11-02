-- T-SQL init script voor revali_db (SQL Server)
-- Run als sysadmin (sa). Maakt DB en tabellen aan als ze nog niet bestaan.

SET NOCOUNT ON;
-- 1) Maak database aan als die niet bestaat
IF DB_ID(N'revali_db') IS NULL
BEGIN
    PRINT N'Creating database revali_db...';
    CREATE DATABASE [revali_db];
    PRINT N'revali_db aangemaakt.';
END
ELSE
    PRINT N'revali_db bestaat al.';

-- 2) Schakel naar de database
USE [revali_db];
GO

------------------------------------------------------------------
-- Helper: kleine routine om objecten conditioneel aan te maken
------------------------------------------------------------------

/*
Opmerking:
- NVARCHAR gebruikt voor Unicode (vergelijkbaar met utf8mb4)
- IDENTITY(1,1) i.p.v. AUTO_INCREMENT
- CHECK constraints ipv ENUM
- JSON-kolommen worden NVARCHAR(MAX) met ISJSON() constraint
*/

------------------------------------------------------------------
-- Tabel: Users (gebruikersaccounts)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL CONSTRAINT UQ_Users_Username UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL,
        DisplayName NVARCHAR(200) NULL,
        CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    );
    PRINT N'Tabel dbo.Users aangemaakt.';
END
ELSE PRINT N'dbo.Users bestaat al.';

------------------------------------------------------------------
-- Tabel: Patients (patiënten)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Patients', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Patients (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        DateOfBirth DATE NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    );
    PRINT N'Tabel dbo.Patients aangemaakt.';
END
ELSE PRINT N'dbo.Patients bestaat al.';

------------------------------------------------------------------
-- Tabel: RevalidationSessions (consulten / sessies)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.RevalidationSessions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RevalidationSessions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        SessionDate DATETIME2(3) NOT NULL,
        Notes NVARCHAR(MAX) NULL,
        Cost DECIMAL(10,2) NOT NULL DEFAULT(0),
        CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_RevalSession_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
            -- NO ACTION is default; expliciet kan ON DELETE NO ACTION
            ON DELETE NO ACTION
    );
    PRINT N'Tabel dbo.RevalidationSessions aangemaakt.';
END
ELSE PRINT N'dbo.RevalidationSessions bestaat al.';

------------------------------------------------------------------
-- Tabel: ExerciseLogs (uitvoeringen van oefeningen)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ExerciseLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExerciseLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        PerformedAt DATETIME2(3) NOT NULL,
        Exercise NVARCHAR(255) NOT NULL,
        Repetitions INT NOT NULL DEFAULT(0),
        Notes NVARCHAR(MAX) NULL,
        CONSTRAINT FK_ExerciseLog_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
            ON DELETE NO ACTION
    );
    PRINT N'Tabel dbo.ExerciseLogs aangemaakt.';
END
ELSE PRINT N'dbo.ExerciseLogs bestaat al.';

------------------------------------------------------------------
-- Tabel: PainLogs (pijnregistratie 0-10)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.PainLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PainLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        RecordedAt DATETIME2(3) NOT NULL,
        PainScore TINYINT NOT NULL,
        Notes NVARCHAR(MAX) NULL,
        CONSTRAINT CK_PainScore_Range CHECK (PainScore BETWEEN 0 AND 10),
        CONSTRAINT FK_Pain_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
            ON DELETE NO ACTION
    );
    PRINT N'Tabel dbo.PainLogs aangemaakt.';
END
ELSE PRINT N'dbo.PainLogs bestaat al.';

------------------------------------------------------------------
-- Tabel: Medication (medicatie)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Medication', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Medication (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        Dosage NVARCHAR(100) NOT NULL,
        StartDate DATE NOT NULL,
        EndDate DATE NULL,
        CONSTRAINT FK_Med_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
            ON DELETE NO ACTION
    );
    PRINT N'Tabel dbo.Medication aangemaakt.';
END
ELSE PRINT N'dbo.Medication bestaat al.';

------------------------------------------------------------------
-- Tabel: Declarations (declaraties / claims)
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Declarations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Declarations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        [Date] DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
        Amount DECIMAL(10,2) NOT NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT(N'Concept'),
            -- status values: Concept, Ingediend, Betaald, Afgewezen
        CONSTRAINT CK_Declarations_Status CHECK ([Status] IN (N'Concept', N'Ingediend', N'Betaald', N'Afgewezen')),
        CONSTRAINT FK_Decl_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
            ON DELETE NO ACTION
    );
    PRINT N'Tabel dbo.Declarations aangemaakt.';
END
ELSE PRINT N'dbo.Declarations bestaat al.';

------------------------------------------------------------------
-- Tabel: AuditEntries (audit / historiek)
-- KeyValues, OldValues, NewValues: NVARCHAR(MAX) met ISJSON-validation
------------------------------------------------------------------
IF OBJECT_ID(N'dbo.AuditEntries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditEntries (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TableName NVARCHAR(200) NOT NULL,
        KeyValues NVARCHAR(MAX) NOT NULL,
        Action NVARCHAR(20) NOT NULL, -- INSERT, UPDATE, DELETE
        OldValues NVARCHAR(MAX) NULL,
        NewValues NVARCHAR(MAX) NULL,
        UserId NVARCHAR(100) NULL,
        [Timestamp] DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_Audit_KeyValues_JSON CHECK (ISJSON(KeyValues) = 1),
        CONSTRAINT CK_Audit_OldValues_JSON CHECK (OldValues IS NULL OR ISJSON(OldValues) = 1),
        CONSTRAINT CK_Audit_NewValues_JSON CHECK (NewValues IS NULL OR ISJSON(NewValues) = 1)
    );
    PRINT N'Tabel dbo.AuditEntries aangemaakt.';
END
ELSE PRINT N'dbo.AuditEntries bestaat al.';

------------------------------------------------------------------
-- Indexes (prestaties)
------------------------------------------------------------------
-- Maak indexes alleen als ze nog niet bestaan
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RevalSession_PatientId' AND object_id = OBJECT_ID(N'dbo.RevalidationSessions'))
BEGIN
    CREATE INDEX IX_RevalSession_PatientId ON dbo.RevalidationSessions (PatientId);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Exercise_PatientId' AND object_id = OBJECT_ID(N'dbo.ExerciseLogs'))
BEGIN
    CREATE INDEX IX_Exercise_PatientId ON dbo.ExerciseLogs (PatientId);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pain_PatientId_RecordedAt' AND object_id = OBJECT_ID(N'dbo.PainLogs'))
BEGIN
    CREATE INDEX IX_Pain_PatientId_RecordedAt ON dbo.PainLogs (PatientId, RecordedAt);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Med_PatientId' AND object_id = OBJECT_ID(N'dbo.Medication'))
BEGIN
    CREATE INDEX IX_Med_PatientId ON dbo.Medication (PatientId);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Decl_PatientId' AND object_id = OBJECT_ID(N'dbo.Declarations'))
BEGIN
    CREATE INDEX IX_Decl_PatientId ON dbo.Declarations (PatientId);
END

PRINT N'Alle tabellen en indices verwerkt.';

GO

--  Verifieer: lijst tabellen
SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME;
GO