SET NOCOUNT ON;

-- 1) Zorg dat login bestaat en default database juist staat
USE master;
IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'revali_login')
BEGIN
    ALTER LOGIN [revali_login] WITH DEFAULT_DATABASE = [revali_db];
END
ELSE
BEGIN
    CREATE LOGIN [revali_login] WITH PASSWORD = N'RevaliUserPassw0rd!';
    ALTER LOGIN [revali_login] WITH DEFAULT_DATABASE = [revali_db];
END
GO

-- 2) Koppel login aan database als user (met default schema dbo)
IF DB_ID(N'revali_db') IS NULL
BEGIN
    RAISERROR('Database [revali_db] bestaat niet. Run eerst mssql_init.sql.', 16, 1);
    RETURN;
END
GO
USE [revali_db];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'revali_login')
    CREATE USER [revali_login] FOR LOGIN [revali_login] WITH DEFAULT_SCHEMA = [dbo];
ELSE
    ALTER USER [revali_login] WITH LOGIN = [revali_login], DEFAULT_SCHEMA = [dbo];

-- Basis CONNECT recht
GRANT CONNECT TO [revali_login];

-- 3) Verwijder brede rollen (alleen lezen uit tabellen gewenst)
IF IS_ROLEMEMBER('db_owner', 'revali_login') = 1 EXEC sp_droprolemember N'db_owner', N'revali_login';
IF IS_ROLEMEMBER('db_datareader', 'revali_login') = 1 EXEC sp_droprolemember N'db_datareader', N'revali_login';
IF IS_ROLEMEMBER('db_datawriter', 'revali_login') = 1 EXEC sp_droprolemember N'db_datawriter', N'revali_login';

-- 4) Maak rol voor alleen SELECT op user tabellen
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE type = 'R' AND name = N'read_tables')
    CREATE ROLE [read_tables] AUTHORIZATION [dbo];

-- Grant SELECT per tabel (geen views/procs)
DECLARE @sql nvarchar(max) = N'';
SELECT @sql = @sql + N'GRANT SELECT ON ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N' TO [read_tables];' + CHAR(10)
FROM sys.tables t
WHERE t.is_ms_shipped = 0;

EXEC sp_executesql @sql;

-- Voeg user toe aan read_tables
IF IS_ROLEMEMBER('read_tables', 'revali_login') <> 1
    EXEC sp_addrolemember N'read_tables', N'revali_login';

-- 5) Nodig voor Row-Level Security usage in de API
GRANT EXECUTE ON OBJECT::sys.sp_set_session_context TO [revali_login];
GO

PRINT 'revali_login is beperkt tot SELECT op tabellen en kan SESSION_CONTEXT zetten.';