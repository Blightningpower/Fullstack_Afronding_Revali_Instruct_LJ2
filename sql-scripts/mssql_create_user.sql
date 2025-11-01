-- Voer dit als ÉÉN batch uit (select all -> Execute)
-- Vereist: je moet serveradmin/sysadmin zijn om CREATE LOGIN te mogen uitvoeren.

DECLARE @loginName sysname = N'revali_login';
DECLARE @dbUserName sysname = N'revali_user';
DECLARE @password nvarchar(128) = N'Revali_pass!2025';

-- 1) maak server-login (op master) met dynamische SQL omdat CREATE LOGIN geen variabele voor PASSWORD accepteert
USE master;
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @loginName)
BEGIN
    DECLARE @sql nvarchar(max) = N'CREATE LOGIN ' + QUOTENAME(@loginName) 
        + N' WITH PASSWORD = @pwd, CHECK_POLICY = ON, CHECK_EXPIRATION = OFF;';
    PRINT 'Creating server login via dynamic SQL: ' + @loginName;
    EXEC sp_executesql @sql, N'@pwd nvarchar(128)', @pwd = @password;
END
ELSE
    PRINT 'Login already exists: ' + @loginName;

-- 2) maak database-user in revali_db (koppelt login aan user)
USE [revali_db];

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @dbUserName)
BEGIN
    PRINT 'Creating database user: ' + @dbUserName;
    CREATE USER [revali_user] FOR LOGIN [revali_login];
END
ELSE
    PRINT 'Database user already exists: ' + @dbUserName;

-- 3) geef minimale rechten: lezen + schrijven (geen db_owner)
PRINT 'Adding user to db_datareader and db_datawriter';
IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datareader' AND m.name = @dbUserName)
BEGIN
    ALTER ROLE db_datareader ADD MEMBER [revali_user];
END

IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datawriter' AND m.name = @dbUserName)
BEGIN
    ALTER ROLE db_datawriter ADD MEMBER [revali_user];
END

-- 4) role voor stored-procedure-execute (optioneel)
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'db_executor' AND type = 'R')
BEGIN
    PRINT 'Creating role db_executor and granting EXECUTE on dbo schema';
    CREATE ROLE [db_executor];
    GRANT EXECUTE ON SCHEMA::dbo TO [db_executor];
END
ELSE
    PRINT 'Role db_executor already exists';

IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_executor' AND m.name = @dbUserName)
BEGIN
    ALTER ROLE [db_executor] ADD MEMBER [revali_user];
END

-- 5) verificatie: laat rollen van de user zien
PRINT 'Membership voor revali_user:';
SELECT r.name AS RoleName, m.name AS MemberName
FROM sys.database_role_members drm
JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
WHERE m.name = @dbUserName;