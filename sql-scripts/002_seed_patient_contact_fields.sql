-- Email genereren op basis van naam
UPDATE P
SET P.Email = LOWER(LEFT(P.FirstName,1) + '.' + REPLACE(P.LastName,' ','') + '@examplehospital.nl')
FROM dbo.Patients P
WHERE P.Email IS NULL AND P.FirstName IS NOT NULL AND P.LastName IS NOT NULL;

-- Telefoon genereren (pseudo 06-nummer)
UPDATE P
SET P.Phone = '06-' + RIGHT('00000000' + CAST(ABS(CHECKSUM(NEWID())) % 100000000 AS VARCHAR(8)), 8)
FROM dbo.Patients P
WHERE P.Phone IS NULL;

-- Verwijzend arts demo
UPDATE P
SET P.ReferringDoctor = 'Huisarts ' + CHAR(65 + ABS(CHECKSUM(NEWID()) % 26))
FROM dbo.Patients P
WHERE P.ReferringDoctor IS NULL;
