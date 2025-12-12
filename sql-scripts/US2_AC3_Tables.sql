---------------------------------------------------------
-- 1. Exercises
---------------------------------------------------------
IF OBJECT_ID('dbo.Exercises', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Exercises (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Code         NVARCHAR(50)  NOT NULL,
        Title        NVARCHAR(200) NOT NULL,
        Description  NVARCHAR(1000) NULL
    );
END
GO

---------------------------------------------------------
-- 2. ExerciseAssignments
---------------------------------------------------------
IF OBJECT_ID('dbo.ExerciseAssignments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExerciseAssignments (
        Id               INT IDENTITY(1,1) PRIMARY KEY,
        PatientId        INT NOT NULL,
        ExerciseId       INT NOT NULL,
        Repetitions      INT NULL,
        Sets             INT NULL,
        Frequency        NVARCHAR(100) NULL,
        Duration         TIME NULL,
        StartDateUtc     DATETIME2(3) NOT NULL,
        EndDateUtc       DATETIME2(3) NULL,
        ClientCheckedOff BIT NOT NULL DEFAULT 0,
        AssignedAtUtc    DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
        AssignedByUserId INT NOT NULL,

        CONSTRAINT FK_ExerciseAssignments_Patients
            FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id),

        CONSTRAINT FK_ExerciseAssignments_Exercises
            FOREIGN KEY (ExerciseId) REFERENCES dbo.Exercises(Id),

        CONSTRAINT FK_ExerciseAssignments_Users
            FOREIGN KEY (AssignedByUserId) REFERENCES dbo.Users(Id)
    );
END
GO

---------------------------------------------------------
-- 3. PainEntries
---------------------------------------------------------
IF OBJECT_ID('dbo.PainEntries', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PainEntries (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        PatientId     INT NOT NULL,
        RecordedAtUtc DATETIME2(3) NOT NULL,
        Score         INT NOT NULL,
        Location      NVARCHAR(200) NULL,
        Note          NVARCHAR(MAX) NULL,

        CONSTRAINT FK_PainEntries_Patients
            FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
    );
END
GO

---------------------------------------------------------
-- 4. ActivityLogs
---------------------------------------------------------
IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ActivityLogs (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        PatientId   INT NOT NULL,
        LoggedAtUtc DATETIME2(3) NOT NULL,
        Activity    NVARCHAR(200) NOT NULL,
        Details     NVARCHAR(MAX) NULL,

        CONSTRAINT FK_ActivityLogs_Patients
            FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id)
    );
END
GO

---------------------------------------------------------
-- 5. Appointments
---------------------------------------------------------
IF OBJECT_ID('dbo.Appointments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Appointments (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        PatientId     INT NOT NULL,
        StartUtc      DATETIME2(3) NOT NULL,
        Duration      TIME NOT NULL,
        Type          NVARCHAR(100) NOT NULL,
        Status        NVARCHAR(50) NOT NULL,   -- Enum als string
        CreatedByUserId INT NOT NULL,
        CreatedAtUtc  DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT FK_Appointments_Patients
            FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id),

        CONSTRAINT FK_Appointments_Users
            FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
    );
END
GO

---------------------------------------------------------
-- 6. AccessoryAdvices
---------------------------------------------------------
IF OBJECT_ID('dbo.AccessoryAdvices', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AccessoryAdvices (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        PatientId           INT NOT NULL,
        GPUserId            INT NOT NULL,
        Name                NVARCHAR(200) NOT NULL,
        AdviceDateUtc       DATETIME2(3) NOT NULL,
        ExpectedUsagePeriod NVARCHAR(100) NOT NULL,
        Status              NVARCHAR(50) NOT NULL,

        CONSTRAINT FK_AccessoryAdvices_Patients
            FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id),

        CONSTRAINT FK_AccessoryAdvices_Users
            FOREIGN KEY (GPUserId) REFERENCES dbo.Users(Id)
    );
END
GO