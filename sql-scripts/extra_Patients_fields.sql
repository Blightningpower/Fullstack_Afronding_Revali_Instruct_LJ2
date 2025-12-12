USE revali_db;
GO

IF COL_LENGTH('dbo.Patients', 'Email') IS NULL
BEGIN
    ALTER TABLE dbo.Patients
        ADD Email NVARCHAR(255) NULL;
END;
GO

IF COL_LENGTH('dbo.Patients', 'Phone') IS NULL
BEGIN
    ALTER TABLE dbo.Patients
        ADD Phone NVARCHAR(50) NULL;
END;
GO

IF COL_LENGTH('dbo.Patients', 'ReferringDoctor') IS NULL
BEGIN
    ALTER TABLE dbo.Patients
        ADD ReferringDoctor NVARCHAR(200) NULL;
END;
GO

SELECT TOP (1000) [Id]
      ,[FirstName]
      ,[LastName]
      ,[DateOfBirth]
      ,[Notes]
      ,[CreatedAt]
      ,[StartDate]
      ,[Status]
      ,[Diagnosis]
      ,[AssignedDoctorUserId]
      ,[Email]
      ,[Phone]
      ,[ReferringDoctor]
  FROM [revali_db].[dbo].[Patients]
